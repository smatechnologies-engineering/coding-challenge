// need to have the elevator constantly moving from 0 -> this.floors
// check if the next floor has any requests before we are moving 
// this will ensure that we are never checking during and if we are between 4 and 5 and a request comes in we won't stop and pick up at 5

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
    // this.currentDestination = 0;
    this.departMap = new Map(); 
    this.requestMap = new Map();

    this.travelInterval = 1500;
    this.stopInterval = 500;
    this.quit = false;
    this.passengerQueue = [];
  }

  stopProcess() {
    this.quit = true;
  }

  move() {

    output('Current floor: ' + this.currentFloor);
   
    if(this.quit && !this.requestMap.size && !this.departMap.size && !this.passengerQueue.length) {
      output('we are done');
      process.exit();
    }

    // check if we are at top floor or bottom floor
    if(this.currentDirection === 1 && this.currentFloor === this.floors - 1) {
      this.currentDirection = -1;
    } else if(this.currentDirection === -1 && this.currentFloor === 0) {
      this.currentDirection = 1;
    }

    // increment floor based on direction - change state to moving
    this.currentFloor += this.currentDirection;
    this.state = 'moving';
    this.checkStop();
  }


  checkStop() {
    // run this.check and will return 2 arrays
    const gettingOn = this.checkOn();
    const gettingOff = this.checkOff();
    // console.log(gettingOn, gettingOff);
    // if either array has a length we know we need to stop at the next floor
    // make function here to board and depart elevator
    if(gettingOn.length || gettingOff.length) {
      // stop the elevator
      this.state = 'stopped'
      this.stop(gettingOn, gettingOff);
    } else {
      // call move again from next floor
      this.move = this.move.bind(this);
      setTimeout(this.move, this.travelInterval);
    }

  }


  // we stop for 1 second
  // we must board and deboard all passengers 
  // if we are stopped at a floor we should be able to check if any other requests to board at this floor have come in
  
  // this.requestMap.get(currentFloor).push({destination, direction, weight});
  stop(gettingOn, gettingOff) {

    // first we should remove everyone from the current departMap
    if(gettingOff.length) {
      output(JSON.stringify(gettingOff) + 'all got off ' + this.currentFloor);
      output('passenger off count : ' + this.passengerCount);
    }
    
    // next we must add all the current requests

    // ** can make a new array to hold the passengers who haven't put their destination request in yet ** //

    if(gettingOn.length) {
      const currentFloorRequests = this.requestMap.get(this.currentFloor.toString());
      for(let i = currentFloorRequests.length - 1; i >= 0; i--) {
        const {direction, weight} = currentFloorRequests[i];
        if(direction === this.currentDirection) {
      
          this.passengerBoard(weight, currentFloorRequests, i);
        }

        
      }
      if(currentFloorRequests.length === 0) this.requestMap.delete(this.currentFloor.toString());

    }

    this.move = this.move.bind(this);
    setTimeout(this.move, this.stopInterval);



  }

  // this can be improved if we change how we store the data in the g et on map -> all up requests in one array / all down in another
  passengerBoard(weight, requestArray, index) {
   
    // add to the passenger queue for passengers who haven't yet chosen a destination
    this.passengerQueue.push(weight);
    
    // delete from the requestmap
    requestArray.splice(index, 1);

  }


  // this could be improved conceptually if I used a queue but I got lazy and didn't want to implement one 
  selectFloor(floor) {
    const weight = this.passengerQueue.shift();
    if(this.departMap.get(floor.toString()) === undefined) {
      this.departMap.set(floor.toString(), []);
    }

    this.departMap.get(floor.toString()).push(weight);
    // output('departed on floor ' + floor)

  }


  // we should keep track of how many passengers are getting off so we can more easily know the weight when 
  // I decide to add that feature
  
  // should return an array of which passengers are getting on and where tf they're going
  checkOff() {
    // this will check if someone needs to get off
    const gettingOff = [];
    if(this.departMap.get(this.currentFloor.toString()) !== undefined) {

      gettingOff.push(...this.departMap.get(this.currentFloor.toString()));
      this.passengerCount += gettingOff.length;
      this.departMap.delete(this.currentFloor.toString());
    }
    return gettingOff;
  }

  checkOn() {
    const gettingOn = [];
    // check if someone has made a request to get on 
      // this leads into does someone need to go in the same direction we are moving 

    if(this.requestMap.get(this.currentFloor.toString()) !== undefined) {
      // returns the array of current requests at the floor
      const currentFloorRequestArray = this.requestMap.get(this.currentFloor.toString());

      // if the person in the array is going the same direction as the elevator then they will get added to getting on array
      for(let i = 0; i < currentFloorRequestArray.length; i++) {
        const {direction} = currentFloorRequestArray[i];
        if(direction === this.currentDirection) {
          gettingOn.push(currentFloorRequestArray[i])
        }
      }
    }
    // output('getting on array : ', JSON.stringify(gettingOn))
    return gettingOn;

  }


  // we need to make sure the map is from Key > value ---> currentFloor -> [{destination, direction}, {destination, direction}]
  request(request) {
    const {currentFloor, direction, weight} = request;

    // ****need to add bounds here****

    if(this.requestMap.get(currentFloor) === undefined) {
      this.requestMap.set(currentFloor, []);
    }
    this.requestMap.get(currentFloor).push({direction, weight});
  }  

}




// ** shouldn't be able to have the same destination and current floor
// ** also shouldn't be able to hit up on the elevator but also go down in the request





module.exports = {Elevator};



