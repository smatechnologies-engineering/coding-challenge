import { validateFloor } from '../src/elevator.request.js';

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
