/* eslint-env jest */

import { logEvent, writeLog, typeOfEvent } from '../src/elevator.log.js';

test('logEvent successfully record an event', () => {
  const mockedTimestamp = '2023-08-30T13:00:00.000Z';

  Date.now = jest.fn(() => new Date(mockedTimestamp).getTime());

  const log = logEvent(typeOfEvent.INSIDE, 5, mockedTimestamp);

  expect(log).toEqual([{
    'timestamp':'2023-08-30T13:00:00.000Z',
    'event':'inside elevator request',
    'floorNumber':5
  }]);
});

