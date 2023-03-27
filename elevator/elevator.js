// to write to new files for output 
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

 
  /* ****** problems with move function
    elevator takes 3s to move from floor to floor

    we need to check who is getting on / off -> then after we do -> we need to either tell the elevator to stop at next floor or continue to the next
    when we get to the next floor -> if we are stopped -> people get on or off -> we then decide next direction / destination
    if we get to next floor and don't stop -> check next floor -> wait 3s to get there -> repeat again 

    the problem with the check destination thing is that we are checking destination before people have gotten on the elevator and the array in reqobj has been deleted
    so it checks the destination and just moves in the opposite direction even though there isn't really any need to stop ? 

    probably can circumvent this by not checking the current req floor array

  */



  // still need to log here;
  move() {
    this.move = this.move.bind(this);

    output('Current Floor: ' + this.currentFloor);
    const changedDirection = this.decideDirection();

    if(this.currentDestination === -1) {
      setTimeout(this.move, this.stopInterval);
      return;
    }

    this.currentFloor += this.currentDirection;

    const gettingOn = this.checkOn();
    output(gettingOn);
    const gettingOff = this.checkOff();

    if(gettingOn.length || gettingOff.length) {
      this.state = 'stopped';
      this.stop(gettingOn, gettingOff);
      return;
    }

    
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
      if(!this.requestObj[this.currentFloor][this.currentDirection].length) {
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
      this.currentDestination = keys.at(-1);
    } else if(this.currentDestination === -1) {
      this.currentDestination = keys[0];
    }

  }

  stop(gettingOn, gettingOff) {


    if(gettingOff.length) {
      output(JSON.stringify(gettingOff) + 'all got off ' + this.currentFloor);
      output('passenger off count : ' + this.passengerCount);
    }

    if(gettingOn.length) {
      this.passengerQueue.push(...gettingOn);
      output(JSON.stringify(gettingOff) + ' all got on ' + this.currentFloor)
    }

    this.move = this.move.bind(this);
    setTimeout(this.move, this.stopInterval);
  }
  
  checkOff() {
    const gettingOff = [];
    // const nextFloor = this.currentFloor + this.currentDirection;
    if(this.departMap.get(this.currentFloor) !== undefined) {

      gettingOff.push(...this.departMap.get(this.currentFloor));
      this.passengerCount += gettingOff.length;
      this.departMap.delete(nextFloor);
    }
    return gettingOff;
  }

  checkOn() {
    const gettingOn = [];
    // const nextFloor = this.currentFloor + this.currentDirection;

    if(this.requestObj[this.currentFloor] !== undefined) {
      gettingOn.push(...this.requestObj[this.currentFloor][this.currentDirection])
      this.requestObj[this.currentFloor][this.currentDirection] = [];
      
      if(!this.requestObj[this.currentFloor][this.currentDirection * -1].length) {
        delete this.requestObj[this.currentFloor];
      }

    }
    
    return gettingOn;
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



const obj = {};

// console.log(obj.keys())
console.log(Object.keys(obj));
// console.log(obj.keys.length)


 // move() {

  //   output('Current floor: ' + this.currentFloor);
   
  //   if(this.quit && !this.requestMap.size && !this.departMap.size && !this.passengerQueue.length) {
  //     output('we are done');
  //     process.exit();
  //   }

  //   if(this.currentDirection === 1 && this.currentFloor === this.floors - 1) {
  //     this.currentDirection = -1;
  //   } else if(this.currentDirection === -1 && this.currentFloor === 0) {
  //     this.currentDirection = 1;
  //   }

  //   this.currentFloor += this.currentDirection;
  //   this.state = 'moving';
  //   this.checkStop();
  // }