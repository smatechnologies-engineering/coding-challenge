import { validateFloor, elevatorTimer } from '../src/elevator.request.js';

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