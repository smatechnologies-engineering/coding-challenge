// to write to new files for output 
const fs = require('fs');

// to add EOL character to file output
const os = require('os');

// to make output easier
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

    if(this.quit && this.endProcess()) {
      process.exit();
    }
    if(this.currentFloor === 0) this.currentDirection = 1;
    else if(this.currentFloor === this.floors) this.currentDirection = -1;

    if(this.state === 'stopped' && this.currentDestination === -1 && this.requestObj[this.currentFloor]) {
      this.checkOn();
    } else if(this.currentDestination === -1) {
      setTimeout(this.move, this.stopInterval);
      return;
    } else if(this.currentDestination === this.currentFloor) {
        this.setDestinationDecision();
    }
      
    if(this.on.length || this.off.length) {
      output('we got on / off')
      this.stop();
      return;
    }

    

    this.currentFloor += this.currentDirection;
    this.state = 'moving';
    this.checkOn();
    this.checkOff();
    
    setTimeout(this.move, this.travelInterval);
  }

  // if elevator is empty and there are still unserved requests -> elevator decides where to go here
  setDestinationDecision() {
    const keys = Object.keys(this.requestObj);
    keys.push(...Array.from(this.departMap.keys()));

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

  stop() {
  
    this.state = 'stopped';

    if(this.off.length) {
      output(JSON.stringify(this.off) + 'all got off ' + this.currentFloor);
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
      console.log(this.departMap.get(this.currentFloor))
      this.departMap.delete(this.currentFloor);
    }
  }

  checkOn() {
    
    if(this.requestObj[this.currentFloor] !== undefined) {
      if(this.currentFloor === this.currentDestination && !this.requestObj[this.currentFloor][this.currentDirection].length) {
        this.currentDirection = this.currentDirection === 1 ? -1 : 1;
      }

      this.on.push(...this.requestObj[this.currentFloor][this.currentDirection])
      this.passengerCount += this.requestObj[this.currentFloor][this.currentDirection].length;
      this.requestObj[this.currentFloor][this.currentDirection] = [];

      if(!this.requestObj[this.currentFloor][1].length && !this.requestObj[this.currentFloor][-1].length) {
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


  request(request) {

    const {currentFloor, direction, weight} = request;
    if(this.requestObj[currentFloor] === undefined) {
      this.requestObj[currentFloor] = {
        1: [],
        '-1': []
      }
    }

    this.requestObj[currentFloor][direction].push(weight);
    
    this.setDestinationExternal(currentFloor);

  }

  stopProcess() {
    this.quit = true;
  }

  endProcess() {
    console.log(this.departMap.size);
    console.log(!Object.keys(this.requestObj).length && !this.passengerQueue.length && !this.departMap.size)
    return (!Object.keys(this.requestObj).length && !this.passengerQueue.length && !this.departMap.size)
  }

}

module.exports = {Elevator};

