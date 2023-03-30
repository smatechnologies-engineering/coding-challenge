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
        this.destination = undefined;
    }
    setDestination(floor) {
        if (this.destination === undefined) {
            this.destination = floor;
        }
    }
}
export default ElevatorRequest;
