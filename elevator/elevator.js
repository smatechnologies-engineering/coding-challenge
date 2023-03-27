// to write to new files for output 
const fs = require('fs');

// to add EOL character to file output
const os = require('os');

// to make output easier
const output = txt => {
  fs.appendFileSync('output.txt', txt + os.EOL);
}

// internal request : buttons pressed from inside the elevator by passengers
// external requets : buttons pressed on floor N going U or D 

const sum = arr => {
  let sum = 0;
  sum = arr.reduce((a,b) => a + b);
  return sum;
}

class Elevator { 
  constructor(floors) {
    this.floors = floors;
    this.state = 'stopped'
    this.currentFloor = 0;
    this.currentDirection = 1;
    this.currentDestination = -1;
    this.departureRequestMap = new Map(); 
    this.externalRequestObject = {};
    this.travelInterval = 1500;
    this.stopInterval = 500;
    this.quit = false;
    this.passengerQueue = [];
    this.boardingPassengers = [];
    this.departingPassengers = [];
    this.currentWeight = 0;
    this.weightLimit = 50;
  }


  move() {

    this.move = this.move.bind(this);
    output('Current Floor: ' + this.currentFloor);

    if(this.quit && this.endProcess()) {
      process.exit();
    }

    // check if elevator is inside the correct boundary
    if(this.currentFloor === 0) this.currentDirection = 1;
    else if(this.currentFloor === this.floors) this.currentDirection = -1;

    
    // if elevator is not moving and someone requests same floor to get on
    if(this.state === 'stopped' && this.currentDestination === -1 && this.externalRequestObject[this.currentFloor]) {
      this.checkOn();
    // if elevator is not moving and has no requests - wait 1s and check for new reqeusts  
    } else if(this.currentDestination === -1) {
      setTimeout(this.move, this.stopInterval);
      return;
    // if we have reached the destination - check for new destination based on unserved requests
    } else if(this.currentDestination === this.currentFloor) {
        this.setDestinationDecision();
    }
     
    // check if anyone is getting on or off at current floor
    if(this.boardingPassengers.length || this.departingPassengers.length) {
      output('we got on / off')
      this.stop();
      return;
    }

    // if at or above weight limit : need to reset destination to only internal requests
    if(this.currentWeight >= this.weightLimit) this.setDestinationDecision(); 
    
    if(this.currentDestination === -1) {
      setTimeout(this.move, this.stopInterval);
      return;
    }
   
    this.currentFloor += this.currentDirection;
    this.state = 'moving';
    this.checkOff();
    this.checkOn();
    
    
    setTimeout(this.move, this.travelInterval);
  }

  // if elevator is empty and there are still unserved requests -> elevator decides where to go here
  // picks furthest away floor in opposite direction - either external floor request or internal elevator request
  setDestinationDecision() {

    const keys = [...Array.from(this.departureRequestMap.keys())];

    //if we are at or above the weight limit don't consider external requests
    if(this.currentWeight < this.weightLimit) keys.push(...Object.keys(this.externalRequestObject));

    // if there are no requests to serve => do nothing;
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
  stop() {
  
    this.state = 'stopped';

    if(this.departingPassengers.length) {
      output(JSON.stringify(this.departingPassengers) + 'all got off ' + this.currentFloor);
    }

    if(this.boardingPassengers.length) {
      this.passengerRequestQueue.push(...this.boardingPassengers);
      output(JSON.stringify(this.boardingPassengers) + ' all got on ' + this.currentFloor)
    }
    output('current weight' + this.currentWeight);

    this.departingPassengers = [];
    this.boardingPassengers = [];
    this.move = this.move.bind(this);
    setTimeout(this.move, this.stopInterval);
  }
  
  // check who is getting off the elevator while it is in transit from floor n to floor n +/- 1
  // decrements weight of passengers getting off
  checkOff() {

    if(this.departureRequestMap.get(this.currentFloor) !== undefined) {
      this.departingPassengers.push(...this.departureRequestMap.get(this.currentFloor));
      
      this.currentWeight -= sum(this.departingPassengers);
      
      this.departureRequestMap.delete(this.currentFloor);
    }
  }

  // checks who is getting on based on direction elevator is traveling and currentWeight of elevator
  checkOn() {
    
    if(this.externalRequestObject[this.currentFloor] !== undefined) {
      // if elevator has reached destination floor it needs to change direction if the request is in the opposite direction
      if(this.currentFloor === this.currentDestination && !this.externalRequestObject[this.currentFloor][this.currentDirection].length) {
        this.currentDirection = this.currentDirection === 1 ? -1 : 1;
      }

      const boardingArr = this.externalRequestObject[this.currentFloor][this.currentDirection];
      for(let i = boardingArr.length - 1; i >= 0; i--) {
        let currentBoarder = boardingArr[i];
        if(currentBoarder + this.currentWeight <= this.weightLimit) {
          this.boardingPassengers.push(currentBoarder);
          boardingArr.pop();
          this.currentWeight += currentBoarder;

        } else {
          console.log('over weight limit ' + this.currentWeight)
        }
      }
      if(!boardingArr.length) this.externalRequestObject[this.currentFloor][this.currentDirection] = [];
      

      // clears request Obj of current floor if there aren't any more requests on this floor
      if(!this.externalRequestObject[this.currentFloor][1].length && !this.externalRequestObject[this.currentFloor][-1].length) {
          delete this.externalRequestObject[this.currentFloor];
      }

    }
  }

  // internal floor request sets their destination in departMap
  selectFloor(floor) {
    const weight = this.passengerRequestQueue.shift();
    if(this.departureRequestMap.get(floor) === undefined) {
      this.departureRequestMap.set(floor, []);
    }

    this.departureRequestMap.get(floor).push(weight);
    this.setDestinationExternal(floor);
    
  }


  // if there is an external request where our currentDestination should change 
  setDestinationExternal(floor) {

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
  request(request) {

    const {currentFloor, direction, weight} = request;
    if(this.externalRequestObject[currentFloor] === undefined) {
      this.externalRequestObject[currentFloor] = {
        1: [],
        '-1': []
      }
    }

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
    return (!Object.keys(this.externalRequestObject).length && !this.passengerRequestQueue.length && !this.departureRequestMap.size)
  }

}

module.exports = {Elevator};

