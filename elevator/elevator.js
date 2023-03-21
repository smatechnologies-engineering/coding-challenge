// need to have the elevator constantly moving from 0 -> this.floors
// check if the next floor has any requests before we are moving 
// this will ensure that we are never checking during and if we are between 4 and 5 and a request comes in we won't stop and pick up at 5

const fs = require('fs');
const os = require('os');


class Elevator { 
  constructor(floors) {
    this.floors = floors;
    this.state = 'stopped'
    this.currentFloor = 0;
    this.currentDirection = 1;
    // this.currentDestination = 0;
    // this.numOfPassengers = 0;
    this.departMap = new Map(); 
    this.requestMap = new Map();

    this.travelInterval = 3000;
    this.stopInterval = 1000;
  }

  move() {
    if(!this.requestMap.size && !this.departMap.size) {
      fs.appendFileSync('output.txt', 'we are done \n');
      return;
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

    // if either array has a length we know we need to stop at the next floor
    // make function here to board and depart elevator
    if(gettingOn.length || gettingOff.length) {
      // stop the elevator
      this.state = 'stopped'
      this.stop(gettingOn, gettingOff);
    } else {
      // call move again from next floor
      this.move = this.move.bind(this);
      setTimeout(this.move, this.travelInterval)
    }
  }


  // we stop for 1 second
  // we must board and deboard all passengers 
  // if we are stopped at a floor we should be able to check if any other requests to board at this floor have come in
  
  // this.requestMap.get(currentFloor).push({destination, direction, weight});
  stop(gettingOn, gettingOff) {

    // console.log(gettingOn);
    // first we should remove everyone from the current departMap
    if(gettingOff.length) {
      // console.log(gettingOff);
      
      let output = 'current floor: ' + this.currentFloor.toString() + ' ' + JSON.stringify(this.departMap.get(this.currentFloor)) + 'all got off';
      fs.appendFileSync('output.txt', output + os.EOL);
      this.departMap.delete(this.currentFloor);

    }

    
    // next we must add all the current requests

    if(gettingOn.length) {
      const currentFloorRequests = this.requestMap.get(this.currentFloor);
      for(let i = currentFloorRequests.length - 1; i >= 0; i--) {
        const {destination, direction, weight, name} = currentFloorRequests[i];

        if(direction === this.currentDirection) {
          this.passengerBoard(destination, weight, currentFloorRequests, i, name);
        }

        
      }
      if(currentFloorRequests.length === 0) this.requestMap.delete(this.currentFloor);

    }

    this.move = this.move.bind(this);
    setTimeout(this.move, this.stopInterval);



  }

  // this can be improved if we change how we store the data in the g et on map -> all up requests in one array / all down in another
  passengerBoard(destination, weight, requestArray, index, name) {
    // add to the depart map
    if(this.departMap.get(destination) === undefined) {
      this.departMap.set(destination, []);
    }

    this.departMap.get(destination).push(name);
    const output = 'current floor: ' + this.currentFloor.toString() + ' ' + JSON.stringify('destination: ' + destination + ' weight: ' + weight + ' name: ' + name + ' got on');
    fs.appendFileSync('output.txt', output + os.EOL);


    // delete from the requestmap
    requestArray.splice(index, 1);


  }


  // we should keep track of how many passengers are getting off so we can more easily know the weight when 
  // I decide to add that feature
  
  // should return an array of which passengers are getting on and where tf they're going
  checkOff() {
    // this will check if someone needs to get off
    const gettingOff = [];
    if(this.departMap.get(this.currentFloor) !== undefined) {

      gettingOff.push(...this.departMap.get(this.currentFloor));
      // this.departMap.delete(this.currentFloor);
    }
    console.log('checkoff getting off: ' + gettingOff);
    return gettingOff;
  }

  checkOn() {
    const gettingOn = [];
    // check if someone has made a request to get on 
      // this leads into does someone need to go in the same direction we are moving 
      if(this.requestMap.get(this.currentFloor) !== undefined) {
        // returns the array of current requests at the floor
        const currentFloorRequestArray = this.requestMap.get(this.currentFloor);
  
        // if the person in the array is going the same direction as the elevator then they will get added to getting on array
        for(let i = 0; i < currentFloorRequestArray.length; i++) {
          const {direction} = currentFloorRequestArray[i];
          if(direction === this.currentDirection) {
            gettingOn.push(currentFloorRequestArray[i])
          }
        }
      }
      return gettingOn;

  }


  // we need to make sure the map is from Key > value ---> currentFloor -> [{destination, direction}, {destination, direction}]
  request(request) {
    const {currentFloor, destination, direction, weight, name} = request;

    // ****need to add bounds here****

    if(this.requestMap.get(currentFloor) === undefined) {
      this.requestMap.set(currentFloor, []);
    }
    this.requestMap.get(currentFloor).push({destination, direction, weight,name});
  }

}



// ** shouldn't be able to have the same destination and current floor
// ** also shouldn't be able to hit up on the elevator but also go down in the request
class Request {
  constructor(currentFloor, destination, direction, weight, name) {
    Object.assign(this, {currentFloor, destination, direction, weight,name})
  }
}


// create new elevator
// create new request
// add to the requestMap with request method
// see if the check works for that floor 


const elevator = new Elevator(5);

// request: currentfloor, destination, direction, weight
const aRequest = new Request(1, 2, 1,0,'shay');
const bRequest = new Request(2, 3, 1,0,'sadie');
const cRequest = new Request(3, 1, -1, 0,'mom');
const dRequest = new Request(1, 4, 1,0,'dad');

elevator.request(aRequest)
elevator.request(bRequest)
elevator.request(cRequest)
elevator.request(dRequest)

fs.writeFileSync('output.txt', '');
elevator.move();




