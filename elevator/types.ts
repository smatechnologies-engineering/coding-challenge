
export type Elevator = {  
  floors: number;
  state: 'stopped' | 'moving';
  currentFloor: number;
  currentDirection: 1 | -1;
  currentDestination: number;
  departureRequestMap: Map<number, number[]>;
  externalRequestObject: ExternalRequestObject;
  travelInterval: number;
  stopInterval: number;
  quit: boolean;
  passengerRequestQueue: number[];
  boardingPassengers: number[];
  departingPassengers: number[];
  currentWeight: number;
  weightLimit: number;
}

export type ExternalRequestObject = {
  [key: number | string] : DirectionObject 
}

export type DirectionObject = {
  '1': number[],
  '-1': number[];
  [key: number]: number[];
}

export type ElevatorRequest = {
  currentFloor: number;
  direction: direction;
  weight: number;
  destination: undefined | number;

}


export type direction = 1 | -1;
