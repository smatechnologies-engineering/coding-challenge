class ElevatorRequest {
  currentFloor: number;
  direction: number;
  weight: number;

  constructor(currentFloor: number, direction: number, weight?:number) {
    this.currentFloor = currentFloor;
    this.direction = direction;
    this.weight = weight !== undefined ? weight : 0;
  }
}

export default ElevatorRequest;







