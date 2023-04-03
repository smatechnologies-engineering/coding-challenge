// Import the Elevator class from your implementation file
import Elevat from '../build/elevator/elevator.js'
// import RequestMap from '../elevator/requestMap'
// import Elevator from '../build/elevator/elevator.js';

jest.useFakeTimers();

describe('Elevator.move', () => {
  let elevator: Elevator;

  beforeEach(() => {
    // Create a new Elevator instance before each test
    elevator = new Elevator(15);
  });

  afterEach(() => {
    jest.clearAllTimers();
  })

  it('should move the elevator to the next floor when the current direction is 1 and current destination > current floor', () => {
    elevator.currentFloor = 0;

    elevator.currentDirection = 1;
    elevator.currentDestination = 4;
    // elevator.externalRequestMap.set();

    elevator.move();

    expect(elevator.currentFloor).toEqual(1);
    expect(elevator.state).toEqual('moving');
    expect(elevator.currentDestination).toEqual(1);
    expect(elevator.currentDirection).toEqual(1);
    expect(elevator.boardingPassengers).toEqual([]);
    expect(elevator.departingPassengers).toEqual([]);
  });

//   it('should stop the elevator and board passengers when there are external requests on the next floor', () => {

//     elevator.currentFloor = 2;
//     elevator.currentDirection = 1;
//     elevator.currentDestination = 3;
//     elevator.externalRequestObject = { 3: { 
//       '1': [25], 
//       '-1': []
//     } };
    
//     elevator.move();

//     expect(elevator.currentFloor).toEqual(3);
//     expect(elevator.state).toEqual('moving');
//     expect(elevator.currentDestination).toEqual(3);
//     expect(elevator.currentDirection).toEqual(1);
//     expect(elevator.boardingPassengers.length).toEqual(1);
//     expect(elevator.departingPassengers).toEqual([]);
//   });

//   it('should stop the elevator and deboard/board passengers when there are passengers with a request to get off on the current floor', () => {

//     elevator.currentFloor = 5;
//     elevator.currentDirection = 1;
//     elevator.currentDestination = 5;
//     elevator.externalRequestObject = {};
//     elevator.boardingPassengers = [10,10,10];
//     elevator.departingPassengers = [10];

//     elevator.move();

//     expect(elevator.currentFloor).toEqual(5);
//     expect(elevator.state).toEqual('stopped');
//     expect(elevator.currentDestination).toEqual(-1);
//     expect(elevator.currentDirection).toEqual(1);
//     expect(elevator.boardingPassengers).toEqual([]);
//     expect(elevator.departingPassengers).toEqual([]);
//     expect(elevator.passengerRequestQueue.length).toEqual(3);
//   });

//   it('should not stop at next floor if there is a request to board but we are at the weight limit', () => {

//     elevator.currentFloor = 4;
//     elevator.currentWeight = 50;
//     elevator.currentDirection = 1;
//     elevator.departureRequestMap.set(8, [10]);
//     elevator.currentDestination = 8;
//     elevator.externalRequestObject = { 5: { 
//       '1': [25], 
//       '-1': []
//     } };

//     elevator.move();

//     expect(elevator.currentFloor).toEqual(5);
//     expect(elevator.state).toEqual('moving');
//     expect(elevator.currentDestination).toEqual(8);
//     expect(elevator.currentDirection).toEqual(1);
//     expect(elevator.boardingPassengers).toEqual([]);
//     expect(elevator.departingPassengers).toEqual([]);
//   });

//   it('should properly track weight going on and off', () => {

//     elevator.currentFloor = 4;
//     elevator.currentWeight = 50;
//     elevator.currentDirection = 1;
//     elevator.currentDestination = 5;
//     elevator.externalRequestObject= {5: {'1': [10,10,10], '-1':[]}}
//     elevator.departureRequestMap.set(5, [10]);
//     elevator.currentWeight = 20;

//     elevator.move();

//     expect(elevator.currentFloor).toEqual(5);
//     expect(elevator.state).toEqual('moving');
//     expect(elevator.currentWeight).toEqual(40);
    
//   });

//   it('should not stop program if elevator.stop is true, it should finish requests and then stop', () => {
//     elevator.quit = true;
//     elevator.currentFloor = 4; 
//     elevator.currentDirection = 1;
//     elevator.currentDestination = 5;
//     elevator.departureRequestMap.set(5,[10]);
    
//     elevator.move();

//     expect(elevator.currentFloor).toEqual(5);
//     expect(elevator.state).toEqual('moving');

//   })


// });





// describe('Elevator.setDestinationExternal', () => {
//   let elevator: Elevator;

//   beforeEach(() => {
//     // Create a new Elevator instance before each test
//     elevator = new Elevator(10);
//   });

//   afterEach(() => {
//     jest.clearAllTimers();
//   })

//   it('should set destination to higher floor if moving up', () => {
//     elevator.currentFloor = 0;
//     elevator.currentDirection = 1;
//     elevator.currentDestination = 4;

//     elevator.setDestinationExternal(9);

//     expect(elevator.currentDestination).toEqual(9);
//   });

//   it('should set destination to lower floor if moving down', () => {
//     elevator.currentFloor = 9;
//     elevator.currentDirection = -1;
//     elevator.currentDestination = 4;

//     elevator.setDestinationExternal(2);

//     expect(elevator.currentDestination).toEqual(2);
//   });

//   it('should not change destination if moving up and currentDest > new request', () => {
//     elevator.currentFloor = 0;
//     elevator.currentDirection = 1;
//     elevator.currentDestination = 4;

//     elevator.setDestinationExternal(3);

//     expect(elevator.currentDestination).toEqual(4);
//   });

//   it('should not change destination if moving down and currentDest < new request', () => {
//     elevator.currentFloor = 9;
//     elevator.currentDirection = -1;
//     elevator.currentDestination = 4;

//     elevator.setDestinationExternal(5);

//     expect(elevator.currentDestination).toEqual(4);
//   });

});