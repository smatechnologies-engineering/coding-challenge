import { logEvent, typeOfEvent } from './elevator.log.js';

const MAX_FLOOR = 10;
const MIN_FLOOR = 1;
const MOVE_DELAY = 300;
const STOP_DELAY = 100;
const maxWeight = 1000;

const elevatorSensor = {
  currentFloor: 1,
  nextFloor: 1,
  requestedFloor: [],
  direction: '',
  isMoving: false,
  weightLimit: false,
}

function validateFloor(floorNum) { 
  if (floorNum > MAX_FLOOR || floorNum < MIN_FLOOR) {
    throw new Error('Invalid floor number');
  }
  return floorNum;
}

async function elevatorTimer(time) {
  return new Promise((resolve) => {
    setTimeout(resolve, time);
  });
}

async function handleRequests() {
  while (elevatorSensor.requestedFloor.length > 0) {   
    if (elevatorSensor.currentFloor < elevatorSensor.nextFloor) {
      elevatorSensor.direction = 'U';
    }
  
    else {
      elevatorSensor.direction = 'D';
    }
    
    elevatorSensor.isMoving = true;

    while (elevatorSensor.nextFloor != elevatorSensor.currentFloor) {
      elevatorSensor.nextFloor = getNextPickup();

      if (elevatorSensor.direction === 'U') {
        elevatorSensor.currentFloor++;
      }

      else {
        elevatorSensor.currentFloor--;
      }
      
      await elevatorTimer(MOVE_DELAY);
    }
    
    elevatorSensor.isMoving = false;

    await elevatorTimer(STOP_DELAY);

    elevatorSensor.requestedFloor.splice(elevatorSensor.requestedFloor.indexOf(elevatorSensor.currentFloor), 1);

    elevatorSensor.nextFloor = getNextPickup();
  }
  
}

function resetElevatorPosition() {

}
function getNextPickup() {
  if (elevatorSensor.requestedFloor.length === 0) {
    return null;
  }

  // Filter floors that match the current direction
  const floorsInDirection = elevatorSensor.requestedFloor.filter(floor => {
    if (elevatorSensor.direction === 'U') {
      return floor > elevatorSensor.currentFloor;
    }

    else {
      return floor < elevatorSensor.currentFloor;
    }
  });

  const floorsToConsider = floorsInDirection.length > 0 ? floorsInDirection : elevatorSensor.requestedFloor;

  const distances = floorsToConsider.map(floor => Math.abs(floor - elevatorSensor.currentFloor));

  const sortedFloors = floorsToConsider.slice().sort((a, b) => distances[floorsToConsider.indexOf(a)] - distances[floorsToConsider.indexOf(b)]);

  elevatorSensor.nextFloor = sortedFloors[0];

  return sortedFloors[0];
}

async function insideElevatorRequest(floorNum) {
  floorNum = validateFloor(floorNum);
  //Timestamp here
  if (elevatorSensor.requestedFloor.length === 0) {
    elevatorSensor.nextFloor = floorNum;
  }
  elevatorSensor.requestedFloor.push(floorNum);

  await (handleRequests());

  return elevatorSensor;
}

async function outsideElevatorRequest(floorNum) {

  validateFloor(floorNum);

  //Timestamp here

  if (elevatorSensor.requestedFloor.length === 0) {
    elevatorSensor.nextFloor = floorNum;
  }

  if (elevatorSensor.isMoving && Math.abs(floorNum - elevatorSensor.currentFloor) >= 1 || !elevatorSensor.isMoving) {
    elevatorSensor.requestedFloor.push(floorNum);
  }

  else {
    elevatorTimer(MOVE_DELAY);
    elevatorSensor.requestedFloor.push(floorNum);
  }
}

module.exports.validateFloor = validateFloor;
module.exports.elevatorTimer = elevatorTimer;

insideElevatorRequest(6);
outsideElevatorRequest(1);
//insideElevatorRequest(7);
outsideElevatorRequest(2);