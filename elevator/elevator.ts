import {ElevatorRequest, DirectionObject, direction} from './types.js';
import RequestMap from './requestMap.js';
import Output from './output.js';

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
  departureRequestMap: RequestMap<number[]>;
  externalRequestMap: RequestMap<DirectionObject>;
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
    this.departureRequestMap = new RequestMap<number[]>();// maybe it makes sense to have this be a priority queue? 
    this.externalRequestMap = new RequestMap<DirectionObject>();
    this.passengerRequestQueue = [];
    this.boardingPassengers = [];
    this.departingPassengers = [];
    this.output = new Output('output.txt');
  }

  move() {

    this.move = this.move.bind(this);

    if(this.quit && this.endProcess()) {
      process.exit();
    }
    
    // check if anyone is getting on or off at current floor
    if(this.boardingPassengers.length || this.departingPassengers.length) {
      this.output.output('Stopped on floor: ' + this.currentFloor);
      this.stopAndBoard();
      return;
    } 

    if(this.state === 'moving') {
      this.output.output('Passing floor: ' + this.currentFloor);
    }

    this.checkDestination(); 
    
    // increment / decrement current floor and change state to moving if there is a current destination
    if(this.currentDestination !== -1) {
      this.currentFloor += this.currentDirection;
      this.state = 'moving';
    }

    // check if anyone is getting on / off at upcoming floor
    this.checkOff();
    this.checkOn();

    
    if(this.state === 'moving') {
      setTimeout(this.move, this.travelInterval);
    } else {
      setTimeout(this.move, this.stopInterval);
    }
    
  }


  // need to chagne it so we aren't taking min of -1 and another number
  checkDestination() {

    let min = this.departureRequestMap.min;
    let max = this.departureRequestMap.max;
    
    //if we are at or above the weight limit don't consider external requests
    if(this.currentWeight < this.weightLimit) {
      if(min === -1) min = this.externalRequestMap.min;
      else if (this.externalRequestMap.min !== -1) min = Math.min(this.externalRequestMap.min, min);
      max = Math.max(this.externalRequestMap.max, max);
    }

    if(min === -1 && max === -1) {
      this.currentDestination = -1;
      return;
    }

    if(this.currentDirection === 1) {
      this.currentDestination = max;
    } else if(this.currentDirection === -1) {
      this.currentDestination = min;
    }

    this.currentDirection = this.currentFloor < this.currentDestination ? 1 : -1;

  }

  // stops elevator and boards / deboards passengers depending on their request
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

    if(this.currentFloor === this.currentDestination){
      this.checkDestination();
    } 

    setTimeout(this.move, this.stopInterval);
  }
  
  // check who is getting off the elevator while it is in transit from floor n to floor n +/- 1
  // decrements weight of passengers getting off
  checkOff() {

    if(this.departureRequestMap.map.get(this.currentFloor) !== undefined) {
      this.departingPassengers.push(...this.departureRequestMap.map.get(this.currentFloor) ?? []);
      this.currentWeight -= sum(this.departingPassengers);
      this.departureRequestMap.delete(this.currentFloor);
    }
  }

  // checks who is getting on based on direction elevator is traveling and currentWeight of elevator
  checkOn() {
    
    if(this.externalRequestMap.map.get(this.currentFloor) !== undefined) {
      // if elevator has reached destination floor it needs to change direction if the request is in the opposite direction
      if(this.currentFloor === this.currentDestination && 
        !this.externalRequestMap.map.get(this.currentFloor)![this.currentDirection].length) {
          this.currentDirection = this.currentDirection === 1 ? -1 : 1;
      }

      const boardingArr = this.externalRequestMap.map.get(this.currentFloor)![this.currentDirection];
      for(let i = boardingArr.length - 1; i >= 0; i--) {
        let currentBoarder = boardingArr[i];
        if(currentBoarder + this.currentWeight <= this.weightLimit) {
          this.boardingPassengers.push(currentBoarder);
          boardingArr.pop();
          this.currentWeight += currentBoarder;
        }
      }
      if(!boardingArr.length) this.externalRequestMap.map.get(this.currentFloor)![this.currentDirection] = [];
      
      // clears request Obj of current floor if there aren't any more requests on this floor
      if(!this.externalRequestMap.map.get(this.currentFloor)![1].length && !this.externalRequestMap.map.get(this.currentFloor)![-1].length) {
          this.externalRequestMap.delete(this.currentFloor);
      }

    }
  }

  // internal floor request sets their destination in departureRequestMap
  selectFloor(floor: number) {
    this.output.output('Floor ' + floor + ' requested internally');

    if(!this.passengerRequestQueue.length) return;

    const weight = this.passengerRequestQueue.shift()!;
    if(this.departureRequestMap.map.get(floor) === undefined) {
      this.departureRequestMap.set(floor, []);
    }

    this.departureRequestMap.map.get(floor)!.push(weight);
    
  }

  // handles external request
  request(request: ElevatorRequest) {
    
    const {currentFloor, direction, weight} = request;

    if(this.externalRequestMap.map.get(currentFloor) === undefined) {
      this.externalRequestMap.set(currentFloor, {
        '1': [],
        '-1': []
      })
    }
    this.externalRequestMap.map.get(currentFloor)![direction].push(weight);
    this.output.output('Floor ' + currentFloor + ' requested externally');

  }

  // when quit is typed into console
  stopProcess() {
    this.quit = true;
  }

  // will end when all requests have been served and all passengers are off the elevator
  endProcess() {
    return (!this.externalRequestMap.map.size && 
    !this.passengerRequestQueue.length && 
    !this.departureRequestMap.map.size && !this.boardingPassengers.length
    && !this.departingPassengers.length)
  }

}

export default Elevator;


