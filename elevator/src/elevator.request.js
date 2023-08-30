const maxFloor = 10;
const minFloor = 1;
const maxWeight = 1000;

const elevatorSensor = {
  currentFloor: 1,
  nextFloor: 1,
  requestedFloor: [],
  direction: '',
  isMoving: false,
  weightLimit: false,
}

function validateFloor(floorNum) { //SLAP
  if (floorNum > maxFloor || floorNum < minFloor || floorNum === elevatorSensor.currentFloor) {
    throw new Error('Invalid floor number');
  }
  return floorNum;
}

function elevatorTimer(time) {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve('resolving');
    }, time);
  });
}

async function elevatorTravel() {

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
      
      await elevatorTimer(300);
    }
    
    elevatorSensor.isMoving = false;

    await elevatorTimer(100);

    elevatorSensor.requestedFloor.splice(elevatorSensor.requestedFloor.indexOf(elevatorSensor.currentFloor), 1);

    elevatorSensor.nextFloor = getNextPickup();
  }
  
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

  // If no matching floors, consider all floors in pickupQueue
  const floorsToConsider = floorsInDirection.length > 0 ? floorsInDirection : elevatorSensor.requestedFloor;

  // Prioritize floors based on distance
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

  await (elevatorTravel());

  return elevatorSensor;
}

//5U for example will be parsed as 5 and U in elevator.service.js first
async function outsideElevatorRequest(floorNum, direction) {

  //Validate floor only after we check the next floor number
  validateFloor(floorNum);

  //Timestamp here

  if (elevatorSensor.requestedFloor.length === 0) {
    elevatorSensor.nextFloor = floorNum;
  }

  elevatorSensor.requestedFloor.push(floorNum);
}

module.exports.validateFloor = validateFloor;
module.exports.elevatorTimer = elevatorTimer;

insideElevatorRequest(6);
outsideElevatorRequest(3);
insideElevatorRequest(7);
outsideElevatorRequest(2);