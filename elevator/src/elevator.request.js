const maxFloor = 10;
const minFloor = 1;
const maxWeight = 1000;

const elevatorSensor = {
  currentFloor: 1,
  nextFloor: 2,
  direction: 'up',
  isMoving: false,
  weightLimit: false,
}

function validateFloor(floorNum) {
  if (floorNum > maxFloor || floorNum < minFloor || floorNum === elevatorSensor.currentFloor) {
    throw new Error('Invalid floor number');
  }
  return floorNum;
}

function insideElevatorRequest(floorNum) {
  validateFloor(floorNum); //SLAP
}
//5U for example will be parsed as 5 and U in elevator.service.js first
function outsideElevatorRequest(floorNum, direction) {
  //Validate floor only after we check the next floor number
  validateFloor(floorNum);
}

module.exports.validateFloor = validateFloor;

