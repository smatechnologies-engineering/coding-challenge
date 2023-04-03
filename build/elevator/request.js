"use strict";
// class ElevatorRequest {
//   currentFloor: number;
//   direction: number;
//   weight: number;
//   destinationFloor: undefined | number;
Object.defineProperty(exports, "__esModule", { value: true });
class ElevatorRequest {
    constructor(currentFloor, direction, weight) {
        this.currentFloor = currentFloor;
        this.direction = direction;
        this.weight = weight;
    }
}
exports.default = ElevatorRequest;
