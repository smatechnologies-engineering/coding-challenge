const fs = require('fs');

const elevatorLogs = [];

const typeOfEvent = {
  INSIDE: 'inside elevator request',
  OUTSIDE: 'outside elevator request',
  MOVE: 'move',
  STOP: 'stop',
};

function getLog() {
  return elevatorLogs;
}

function clearLog() {
  elevatorLogs.length = 0;
}

function logEvent(event, floorNumber, timestamp = new Date().toISOString()) {
  elevatorLogs.push({ timestamp, event, floorNumber });
  
  return elevatorLogs;
}

function writeLog() {
  const log = JSON.stringify(elevatorLogs, null, 2);

  fs.writeFileSync('./elevator.log.json', log);
}

module.exports.typeOfEvent = typeOfEvent;
module.exports.writeLog = writeLog;
module.exports.logEvent = logEvent;
module.exports.getLog = getLog;
module.exports.clearLog = clearLog;