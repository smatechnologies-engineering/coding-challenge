const {Elevator} = require('./elevator');
const {Request} = require('./request');
const fs = require('fs');
const readline = require('readline');
const os = require('os');

const output = txt => {
  fs.appendFileSync('output.txt', txt + os.EOL);
}
// Create an interface for reading from the command line
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});


const elevator = new Elevator(15);
elevator.move();

const question = () => {

  if(elevator.quit) {
    if(elevator.passengerQueue.length || Object.keys(elevator.requestObj)) {
      rl.question('No more outside requests at this time. Please input which floor is your destination', (answer) => {
        elevator.selectFloor(Number(answer));
        question()
      })
      
    } else {
      rl.close();
    }
  }
  
  rl.question('What floor are you on and which direction are you going, OR which floor do you want to request?', (answer) => {
    if(answer.at(-1).toLowerCase() === 'u') {
      createRequest(answer.slice(0,-1), 1);
    } else if (answer.at(-1).toLowerCase() === 'd') {
      createRequest(answer.slice(0,-1), -1);
    } else if(answer === 'quit') {
      elevator.stopProcess();
    } else {
      if(elevator.passengerRequestQueue.length) {
        elevator.selectFloor(Number(answer));
      }
    
    }
    question()});
}

// defaulting to weight of 25
const createRequest = (floor, direction) => {
  const req = new Request(Number(floor), direction, 25);
  elevator.request(req);
}


fs.writeFileSync('output.txt', '');
question();



