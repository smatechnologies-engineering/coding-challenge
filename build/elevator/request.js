class ElevatorRequest {
    constructor(currentFloor, direction, weight) {
        this.currentFloor = currentFloor;
        this.direction = direction;
        this.weight = weight !== undefined ? weight : 0;
    }
}
export default ElevatorRequest;
