
import {
  PriorityQueue,
  MinPriorityQueue,
  MaxPriorityQueue,
  ICompare,
  IGetCompareValue,
} from '@datastructures-js/priority-queue';

import Request from './request.js';
import {ElevatorRequest} from './types.js'



class ElevatorQueue {
  queue: PriorityQueue<ElevatorRequest>


  constructor(minMax: 'min' | 'max') {
    this.queue = this.buildQueue(minMax);
  }

  buildQueue(minMax: 'min' | 'max') {
    if(minMax === 'min') {
      return new MinPriorityQueue<ElevatorRequest>(this.compare);
    } 
    return new MaxPriorityQueue<ElevatorRequest>(this.compare);
  }

  compare (request: ElevatorRequest) {
    if(request.destination === undefined) {
      return request.currentFloor;
    }
    return request.destination;
  }







}

export default ElevatorQueue;

