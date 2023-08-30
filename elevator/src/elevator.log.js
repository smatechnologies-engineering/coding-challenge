import fs from 'fs';

const elevatorLogs = [];

const typeOfEvent = {
  INSIDE: 'inside request',
  OUTSIDE: 'outside request',
  MOVE: 'move',
  STOP: 'stop',
}

function logEvent(event, floorNum) {
  const timestamp = new Date().toLocaleTimeString();
  elevatorLogs.push({timestamp, event, floorNum });
  return elevatorLogs;
}

function writeLog() {
  const log = JSON.stringify(elevatorLogs, null, 2);
  fs.writeFileSync('./elevator.log.json', log);
}

module.exports.typeOfEvent = typeOfEvent;
module.exports.writeLog = writeLog;
module.exports.logEvent = logEvent;