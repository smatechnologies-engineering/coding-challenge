const readline = require('readline');
const elevatorRequest = require('./elevator.request.js');
let completed = false;

function handleUserInput(floorNumber) {
  console.log('Floor Number:', floorNumber);

  if (floorNumber === 'Q' || floorNumber === 'q') {
    console.log('Quitting...');

    completed = true;

    return;
  }

  if (!isNaN(floorNumber)) {
    console.log('Inside elevator request');

    elevatorRequest.insideElevatorRequest(parseInt(floorNumber));
  } 

  else {
    console.log('Outside elevator request');

    elevatorRequest.outsideElevatorRequest(parseInt(floorNumber.replace(/\D/g, '')));
  }
}

const userInterface = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
});

async function processUserInput() {
  while (completed === false) {
    const userInput = await new Promise((resolve) => {
      userInterface.question('Please enter console command: ', resolve);
    });

    handleUserInput(userInput);

  }
  const allCompleted = await elevatorRequest.allRequestsCompleted();

  console.log('All requests completed:', allCompleted);

  userInterface.close(); 
}

processUserInput().catch((error) => {
  console.error('An error occurred:', error);
});
