const elevatorLogging = require('../src/elevator.log.js');
import { resetElevatorPosition, insideElevatorRequest, outsideElevatorRequest, validateFloor, elevatorTimer, allRequestsCompleted } from '../src/elevator.request.js';
import { typeOfEvent, getLog, clearLog } from '../src/elevator.log.js';

test('canary', () => {
  expect(true).toBe(true);
});

test('validateFloor request returns floor number', () => {
  expect(validateFloor(5)).toBe(5);
});

test('validateFloor request throws error going past max floor', () => {
  expect(() => validateFloor(11)).toThrow('Invalid floor number');
});

test('validateFloor request throws error going past min floor', () => {
  expect(() => validateFloor(0)).toThrow('Invalid floor number');
});

test('elevatorTimer given 10ms await 10ms', async () => {
  const startTime = Date.now();
  
  await elevatorTimer(10);
  
  const endTime = Date.now();

  const elapsedSeconds = (endTime - startTime) / 1000;

  expect(elapsedSeconds).toBeCloseTo(.01, 1);
});

test('elevatorTimer given 100ms await 100ms', async () => {
  const startTime = Date.now();
  
  await elevatorTimer(100);
  
  const endTime = Date.now();

  const elapsedSeconds = (endTime - startTime) / 1000;

  expect(elapsedSeconds).toBeCloseTo(.1, 1);
});

test('elevator made a request inside return correct output', async () => { 
  await insideElevatorRequest(3);

  const log = getLog();

  expect(log).toEqual([
      expect.objectContaining({ event: typeOfEvent.INSIDE, floorNumber: 3 }),
      expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 2 }),
      expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 3 }),
      expect.objectContaining({ event: typeOfEvent.STOP, floorNumber: 3 })
    ]);
});

test('elevator made a request inside return correct output', async () => { 
  await resetElevatorPosition();

  clearLog();

  await outsideElevatorRequest(3);

  const log = getLog();

  expect(log).toEqual([
    expect.objectContaining({ event: typeOfEvent.OUTSIDE, floorNumber: 3 }),
    expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 2 }),
    expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 3 }),
    expect.objectContaining({ event: typeOfEvent.STOP, floorNumber: 3 })
  ]);
});

test('elevator made a request inside and outside, inside request is processed first', async () => {
  await resetElevatorPosition();

  clearLog();

  await insideElevatorRequest(3);

  await outsideElevatorRequest(1);

  const log = getLog();

  expect(log).toEqual([
    expect.objectContaining({ event: typeOfEvent.INSIDE, floorNumber: 3 }),
    expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 2 }),
    expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 3 }),
    expect.objectContaining({ event: typeOfEvent.STOP, floorNumber: 3 }),
    expect.objectContaining({ event: typeOfEvent.OUTSIDE, floorNumber: 1 }),
    expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 2 }),
    expect.objectContaining({ event: typeOfEvent.MOVE, floorNumber: 1 }),
    expect.objectContaining({ event: typeOfEvent.STOP, floorNumber: 1 })
  ]);
}); 

test('allRequestsCompleted called writeLog', async () => {
  const writeLogSpy = jest.spyOn(elevatorLogging, 'writeLog');

  await allRequestsCompleted();
  
  expect(writeLogSpy).toHaveBeenCalled();
});
