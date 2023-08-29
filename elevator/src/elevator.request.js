const maxFloor = 10;
const minFloor = 1;
const maxWeight = 1000;

const elevatorSensor = {
  currentFloor: 1,
  nextFloor: 1,
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

async function insideElevatorRequest(floorNum) {
  floorNum = validateFloor(floorNum);

  elevatorSensor.nextFloor = floorNum;

  return elevatorSensor;
}

//5U for example will be parsed as 5 and U in elevator.service.js first
function outsideElevatorRequest(floorNum, direction) {
  //Validate floor only after we check the next floor number
  validateFloor(floorNum);
}

module.exports.validateFloor = validateFloor;
module.exports.elevatorTimer = elevatorTimer;
