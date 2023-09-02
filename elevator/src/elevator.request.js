"use strict"; 

const elevatorLogging = require('./elevator.log.js');

const typeOfEvent = elevatorLogging.typeOfEvent;

const MAX_FLOOR = 10;
const MIN_FLOOR = 1;
const MOVE_DELAY = 300;
const STOP_DELAY = 100;

const elevatorSensor = {
  currentFloor: 1,
  nextFloor: null,
  requestedFloor: [],
  direction: '',
  isMoving: false,
}

async function allRequestsCompleted() {
  while (elevatorSensor.requestedFloor.length > 0) {
    await new Promise((resolve) => {
      setTimeout(resolve, 100); 
    });
  }

  // Reset elevator position to floor 1
  await resetElevatorPosition();
  
  elevatorLogging.writeLog();

  return true;
}

function validateFloor(floorNumber) { 
  if (floorNumber > MAX_FLOOR || floorNumber < MIN_FLOOR) {
    throw new Error('Invalid floor number');
  }
  return floorNumber;
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

      elevatorLogging.logEvent(typeOfEvent.MOVE, elevatorSensor.currentFloor);
    }
    
    elevatorSensor.isMoving = false;

    await elevatorTimer(STOP_DELAY);

    elevatorLogging.logEvent(typeOfEvent.STOP, elevatorSensor.currentFloor);

    elevatorSensor.requestedFloor.splice(elevatorSensor.requestedFloor.indexOf(elevatorSensor.currentFloor), 1);

    elevatorSensor.nextFloor = getNextPickup();
  }
}

async function resetElevatorPosition() {
  elevatorSensor.requestedFloor = [1];

  elevatorSensor.nextFloor = 1;

  await handleRequests();

  elevatorSensor.nextFloor = null;

  elevatorSensor.direction = '';
}

function getNextPickup() {
  if (elevatorSensor.requestedFloor.length === 0) {
    return null;
  }

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

async function insideElevatorRequest(floorNumber) {
  floorNumber = validateFloor(floorNumber);

  elevatorLogging.logEvent(typeOfEvent.INSIDE, floorNumber);

  if (elevatorSensor.requestedFloor.length === 0) {
    elevatorSensor.nextFloor = floorNumber;
  }
  elevatorSensor.requestedFloor.push(floorNumber);

  await (handleRequests());
}

async function outsideElevatorRequest(floorNumber) {
  validateFloor(floorNumber);

  elevatorLogging.logEvent(typeOfEvent.OUTSIDE, floorNumber);

  if (elevatorSensor.requestedFloor.length === 0) {
    elevatorSensor.nextFloor = floorNumber;
  }

  if (elevatorSensor.isMoving && Math.abs(floorNumber - elevatorSensor.currentFloor) >= 1 || !elevatorSensor.isMoving) {
    elevatorSensor.requestedFloor.push(floorNumber);
  }

  else {
    elevatorTimer(MOVE_DELAY);

    elevatorSensor.requestedFloor.push(floorNumber);
  }
  await (handleRequests());
}

module.exports.validateFloor = validateFloor;
module.exports.elevatorTimer = elevatorTimer;
module.exports.insideElevatorRequest = insideElevatorRequest;
module.exports.outsideElevatorRequest = outsideElevatorRequest;
module.exports.allRequestsCompleted = allRequestsCompleted;
module.exports.resetElevatorPosition = resetElevatorPosition;
