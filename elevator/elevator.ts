// to write to new files for output 
import * as fs from 'fs'

// to add EOL character to file output
import * as os from 'os';

import {ElevatorRequest, ExternalRequestObject, direction} from './types.js';

import Output from './output.js';

// I think that in order to limit inefficient destination checks we should use a priority queue only for departureMap.
// this way we can keep track of the max or min value depending on direction and peek it in constant time instead of creating
// a whole new array and using O(n) to find the min or max.


const sum = (arr: number[]) => {
  let sum = 0;
  sum = arr.reduce((a, b) => a + b);
  return sum;
};


// internal request : buttons pressed from inside the elevator by passengers
// external requets : buttons pressed on floor N going U or D 

class Elevator { 

  floors: number;
  state: 'stopped' | 'moving';
  currentFloor: number;
  currentDirection: direction;
  currentDestination: number;
  travelInterval: number;
  stopInterval: number;
  quit: boolean;
  currentWeight: number;
  weightLimit: number;
  departureRequestMap: Map<number, number[]>;
  externalRequestObject: ExternalRequestObject;
  passengerRequestQueue: number[];
  boardingPassengers: number[];
  departingPassengers: number[];
  output: Output;

  constructor(floors: number) {
    this.floors = floors;
    this.state = 'stopped'
    this.currentFloor = 0;
    this.currentDirection = 1;
    this.currentDestination = -1;
    this.travelInterval = 3000;
    this.stopInterval = 1000;
    this.quit = false;
    this.currentWeight = 0;
    this.weightLimit = 50;
    this.departureRequestMap = new Map(); // maybe it makes sense to have this be a priority queue? 
    this.externalRequestObject = {};
    this.passengerRequestQueue = [];
    this.boardingPassengers = [];
    this.departingPassengers = [];
    this.output = new Output('output.txt');
  }



  move() {

    this.move = this.move.bind(this);
    
    this.newFloorChecks();
    
    // if at or above weight limit : need to reset destination to only internal requests
    // * can we limit this in any way? 
    if(this.currentWeight >= this.weightLimit) {
      this.setDestinationDecision(); 
    }

   
    this.output.output('Passing floor: ' + this.currentFloor);
    
    // increment / decrement current floor and change state to moving : check if any board / deboard next floor
    this.currentFloor += this.currentDirection;
    this.state = 'moving';

    this.checkOff();
    this.checkOn();
    
    
    setTimeout(this.move, this.travelInterval);
  }

  newFloorChecks() {
    if(this.quit && this.endProcess()) {
      process.exit();
    }

    // check if elevator is inside the correct boundary
    if(this.currentFloor === 0) this.currentDirection = 1;
    else if(this.currentFloor === this.floors) this.currentDirection = -1;

    // if elevator is not moving and someone requests same floor to get on
    if(this.state === 'stopped' && (this.currentDestination === -1 || this.currentDestination === this.currentFloor)
    && this.externalRequestObject[this.currentFloor]) {
      this.checkOn();
    } 
     
    // check if anyone is getting on or off at current floor
    if(this.boardingPassengers.length || this.departingPassengers.length) {
      this.output.output('Stopped on floor: ' + this.currentFloor);
      this.stopAndBoard();
      return;
    } else if(this.currentDestination === -1) {
      setTimeout(this.move, this.stopInterval);
      return;
    }

    
  }


  // *** use a priority queue here *** 

  // pick destination if elevator reached current destination or elevator reached weight limit
  setDestinationDecision() {

    const keys = Array.from(this.departureRequestMap.keys());
    //if we are at or above the weight limit don't consider external requests
    if(this.currentWeight < this.weightLimit) {
      keys.push(Number(...Object.keys(this.externalRequestObject))); 
    }
    
    // if there are no requests to serve
    if(!keys.length) {
      this.currentDestination = -1; 
      return;
    }

    if(this.currentDirection === 1) {
      this.currentDestination = Number(Math.max(...keys));
    } else if(this.currentDirection === -1) {
      this.currentDestination = Number(Math.min(...keys));
    }


    this.currentDirection = this.currentFloor < this.currentDestination ? 1 : -1;

  }

  // stops elevator and boards / deboards passengers depending on their request
  // ** maybe should add functionality to see if people made the request just too late for initial check
  stopAndBoard() {
  
    this.state = 'stopped';

    if(this.departingPassengers.length) {
      this.output.output(this.departingPassengers.length + ' passenger(s) got off ');
    }

    if(this.boardingPassengers.length) {
      this.passengerRequestQueue.push(...this.boardingPassengers);
      this.output.output((this.boardingPassengers.length + ' passenger(s) got on '));
    }

    this.departingPassengers = [];
    this.boardingPassengers = [];
    setTimeout(this.move, this.stopInterval);
  }
  
  // check who is getting off the elevator while it is in transit from floor n to floor n +/- 1
  // decrements weight of passengers getting off
  checkOff() {

    if(this.departureRequestMap.get(this.currentFloor) !== undefined) {
      this.departingPassengers.push(...this.departureRequestMap.get(this.currentFloor) ?? []);
      
      this.currentWeight -= sum(this.departingPassengers);
      
      this.departureRequestMap.delete(this.currentFloor);
      
    }
  }

  // checks who is getting on based on direction elevator is traveling and currentWeight of elevator
  checkOn() {
    
    if(this.externalRequestObject[this.currentFloor] !== undefined) {
      // if elevator has reached destination floor it needs to change direction if the request is in the opposite direction
      if(this.currentFloor === this.currentDestination && 
        !this.externalRequestObject[this.currentFloor][this.currentDirection].length) {
          this.currentDirection = this.currentDirection === 1 ? -1 : 1;
      }

      const boardingArr = this.externalRequestObject[this.currentFloor][this.currentDirection];
      for(let i = boardingArr.length - 1; i >= 0; i--) {
        let currentBoarder = boardingArr[i];
        if(currentBoarder + this.currentWeight <= this.weightLimit) {
          this.boardingPassengers.push(currentBoarder);
          boardingArr.pop();
          this.currentWeight += currentBoarder;
        }
      }
      if(!boardingArr.length) this.externalRequestObject[this.currentFloor][this.currentDirection] = [];
      

      // clears request Obj of current floor if there aren't any more requests on this floor
      if(!this.externalRequestObject[this.currentFloor][1].length && !this.externalRequestObject[this.currentFloor][-1].length) {
          delete this.externalRequestObject[this.currentFloor];
      }

    }
  }

  // internal floor request sets their destination in departureRequestMap
  selectFloor(floor: number) {
    this.output.output('Floor ' + floor + ' requested internally');
    const weight = this.passengerRequestQueue.shift()!;
    if(this.departureRequestMap.get(floor) === undefined) {
      this.departureRequestMap.set(floor, []);
    }

    this.departureRequestMap.get(floor)!.push(weight);
    this.setDestinationExternal(floor);
    
  }


  // if there is an external request where our currentDestination should change 
  setDestinationExternal(floor: number) {

    if(this.currentWeight >= this.weightLimit) return;

    if(this.currentDestination === -1) {
      this.currentDestination = floor;
      this.currentDirection = floor > this.currentFloor ? 1 : -1;
    } else if(this.currentDirection === 1) {
      if(floor > this.currentDestination) this.currentDestination = floor;
    } else if(this.currentDirection === -1) {
      if(floor < this.currentDestination) this.currentDestination = floor;
    }
  }

  // handles external request
  request(request: ElevatorRequest) {
    
    const {currentFloor, direction, weight} = request;
    if(this.externalRequestObject[currentFloor] === undefined) {
      this.externalRequestObject[currentFloor] = {
        '1': [],
        '-1': []
      };
    }
    this.output.output('Floor ' + currentFloor + ' requested externally');

    this.externalRequestObject[currentFloor][direction].push(weight);
    
    // if the destination needs to be changed based on current direction it will be set here if weight allows
    if(this.currentWeight < this.weightLimit) this.setDestinationExternal(currentFloor);

  }

  // when quit is typed into console
  stopProcess() {
    this.quit = true;
  }

  // will end when all requests have been served and all passengers are off the elevator
  endProcess() {
    return (!Object.keys(this.externalRequestObject).length && 
    !this.passengerRequestQueue.length && 
    !this.departureRequestMap.size && !this.boardingPassengers.length
    && !this.departingPassengers.length)
  }

}

export default Elevator;


