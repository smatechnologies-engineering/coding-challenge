class Request {
  constructor(currentFloor, direction, weight = 0) {
    Object.assign(this, {currentFloor, direction, weight})
    this.destination = undefined;
  }

  floorRequest(floor) {
    if(this.destination === undefined) {
      this.destination = floor;
    }
  }
}

module.exports = {Request};