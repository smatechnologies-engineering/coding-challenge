// to write to new files for output 
const { ifError } = require('assert');
const fs = require('fs');

// to add EOL character to file output
const os = require('os');


const output = txt => {
  fs.appendFileSync('output.txt', txt + os.EOL);
}

class Elevator { 
  constructor(floors) {
    this.floors = floors;
    this.state = 'stopped'
    this.currentFloor = 0;
    this.currentDirection = 1;
    this.passengerCount = 0;
    this.currentDestination = -1;
    this.departMap = new Map(); 
    this.requestObj = {};
    this.travelInterval = 1500;
    this.stopInterval = 500;
    this.quit = false;
    this.passengerQueue = [];

    this.on = [];
    this.off = [];
  }


  move() {
    this.move = this.move.bind(this);
    output('Current Floor: ' + this.currentFloor);
    output('Current Dest: ' + this.currentDestination);
    output('Current Dir: ' + this.currentDirection);
    this.decideDirection();
    this.decideDestination();

    if(this.state === 'stopped' && this.currentDestination === -1 && this.requestObj[this.currentFloor]) {
      if(this.requestObj[this.currentFloor][1].length) { // default to up if there is a decision between the two
        this.currentDirection = 1;
      } else if(this.requestObj[this.currentFloor][-1].length) {
        this.currentDirection = -1;
      }
      this.checkOn();
      this.stop();
      return;
    } else if(this.on.length || this.off.length) {
      output('we got on');
      this.stop();
      return;
    }

    

    if(this.currentDestination === -1) {
      setTimeout(this.move, this.stopInterval);
      return;
    }

    
    
    this.currentFloor += this.currentDirection;
    this.state = 'moving';
    this.checkOn();
    this.checkOff();
    this.decideDestination();
    
    setTimeout(this.move, this.travelInterval);
  }
  
  decideDirection() {

    // we must change direction here because we are at the top or bottom floor
    if(this.currentFloor === 0) {
      this.currentDirection = 1; 
    } else if(this.currentFloor === this.floors) {
      this.currentDirection = -1;
    } else if(this.currentDestination === this.currentFloor) {

      // if there are no requests going in the same direction as we were just traveling at target floor
      if(this.requestObj[this.currentFloor] && !this.requestObj[this.currentFloor][this.currentDirection].length) {
        this.currentDirection = this.currentDirection === 1 ? -1 : 1;
      }
    }
  }

  // decides destination when we have reached current destination
  // we also may need to keep track if there have been any requests made in the wrong direction? 
  decideDestination() {
    const keys = Object.keys(this.requestObj);
    keys.push(...Array.from(this.departMap.keys()));
    keys.sort((a,b) => a-b);

    // if there are no requests to serve => do nothing;
    if(!keys.length) {
      this.currentDestination = -1; 
      return;
    }

    if(this.currentDirection === 1) {
      this.currentDestination = Number(keys.at(-1));
    } else if(this.currentDestination === -1) {
      this.currentDestination = Number(keys[0]);
    }

    if(this.currentFloor < this.currentDestination) {
      this.currentDirection = 1;
    } else if(this.currentFloor > this.currentDestination) {
      this.currentDirection = -1;
    }

  }

  stop() {
  
    this.state = 'stopped';

    if(this.off.length) {
      output(JSON.stringify(this.off) + 'all got off ' + this.currentFloor);
      output('passenger off count : ' + this.passengerCount);
    }

    if(this.on.length) {
      this.passengerQueue.push(...this.on);
      output(JSON.stringify(this.on) + ' all got on ' + this.currentFloor)
    }

    this.off = [];
    this.on = [];
    this.move = this.move.bind(this);
    setTimeout(this.move, this.stopInterval);
  }
  
  checkOff() {

    if(this.departMap.get(this.currentFloor) !== undefined) {
      this.off.push(...this.departMap.get(this.currentFloor));
      this.passengerCount -= this.off.length;
      this.departMap.delete(this.currentFloor);
    }
  }

  checkOn() {
    
    if(this.requestObj[this.currentFloor] !== undefined) {
      if(this.currentFloor === this.currentDestination) {
        this.decideDirection();
      }

      this.on.push(...this.requestObj[this.currentFloor][this.currentDirection])
      this.passengerCount += this.requestObj[this.currentFloor][this.currentDirection].length;
      this.requestObj[this.currentFloor][this.currentDirection] = [];
      if(!this.requestObj[this.currentFloor][this.currentDirection * -1].length && !this.requestObj[this.currentFloor][this.currentDirection].length) {
        delete this.requestObj[this.currentFloor];
      }

    }
  }


  selectFloor(floor) {
    const weight = this.passengerQueue.shift();
    if(this.departMap.get(floor) === undefined) {
      this.departMap.set(floor, []);
    }

    this.departMap.get(floor).push(weight);

  }


  request(request) {

    const {currentFloor, direction, weight} = request;
    if(this.requestObj[currentFloor] === undefined) {
      this.requestObj[currentFloor] = {
        1: [],
        '-1': []
      }
    }

    this.requestObj[currentFloor][direction].push(weight);

    // if elevator has no current destination -> set request currentlFloor to currentDestination
    // set currentDirection in right direction as well 
    if(this.currentDestination === -1) {
      this.currentDestination = currentFloor;
      if(currentFloor < this.currentDestination) {
        this.currentDirection = 1;
      } else if(currentFloor > this.currentDestination) {
        this.currentDirection = -1;
      }
      return;
    }
    
    // change current destination if new floor request is further up or down in correct direction as elevator is currently traveling
    if(this.currentDirection === 1) {
      if(currentFloor > this.currentDestination) this.currentDestination = currentFloor;
    } else if(this.currentDirection === -1) {
      if(currentFloor < this.currentDestination) this.currentDestination = currentFloor;
    }

  }

  stopProcess() {
    this.quit = true;
  }

}

module.exports = {Elevator};



