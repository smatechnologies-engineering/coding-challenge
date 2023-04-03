// class ElevatorRequest {
//   currentFloor: number;
//   direction: number;
//   weight: number;
//   destinationFloor: undefined | number;

//   constructor(currentFloor: number, direction: number, weight?:number) {
//     this.currentFloor = currentFloor;
//     this.direction = direction;
//     this.weight = weight !== undefined ? weight : 0;
//     this.destinationFloor = undefined;
//   }

//   setDestination(floor: number) {
//     if(this.destinationFloor === undefined) {
//       this.destinationFloor = floor;
//     }
//   }


// }

import {direction} from './types.js';

class ElevatorRequest  {

  currentFloor: number;
  direction: direction;
  weight: number;

  

  constructor(currentFloor: number, direction: direction, weight:number) {
    this.currentFloor = currentFloor;
    this.direction = direction;
    this.weight = weight;

  }
}

export default ElevatorRequest;







