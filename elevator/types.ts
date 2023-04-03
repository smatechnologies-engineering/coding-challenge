
export type ExternalRequestObject = {
  [key: number | string] : DirectionObject 
}

export type DirectionObject = {
  '1': number[],
  '-1': number[];
  [key: string|number]: number[];
}

export type ElevatorRequest = {
  currentFloor: number;
  direction: direction;
  weight: number;

}


export type direction = 1 | -1;
