// class ElevatorRequest {
//   currentFloor: number;
//   direction: number;
//   weight: number;
//   destinationFloor: undefined | number;
class ElevatorRequest {
    constructor(currentFloor, direction, weight) {
        this.currentFloor = currentFloor;
        this.direction = direction;
        this.weight = weight;
    }
}
export default ElevatorRequest;
