import Elevator from './elevator.js';
import ElevatorRequest from './request.js';
import * as fs from 'fs';
import * as readline from 'readline';
// Create an interface for reading from the command line
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});
const elevator = new Elevator(15);
elevator.move();
const question = () => {
    if (elevator.quit) {
        if (elevator.passengerRequestQueue.length || Object.keys(elevator.externalRequestMap)) {
            rl.question('No more outside requests at this time. Please input which floor is your destination', (answer) => {
                elevator.selectFloor(Number(answer));
                question();
            });
        }
        else {
            rl.close();
        }
    }
    rl.question('What floor are you on and which direction are you going, OR which floor do you want to request?', (answer) => {
        if (!answer.length)
            question();
        if (answer.at(-1).toLowerCase() === 'u') {
            createRequest(answer.slice(0, -1), 1);
        }
        else if (answer.at(-1).toLowerCase() === 'd') {
            createRequest(answer.slice(0, -1), -1);
        }
        else if (answer.toLowerCase() === 'q') {
            elevator.stopProcess();
        }
        else {
            if (elevator.passengerRequestQueue.length) {
                elevator.selectFloor(Number(answer));
            }
        }
        question();
    });
};
// defaulting to weight of 25
const createRequest = (floor, direction) => {
    const req = new ElevatorRequest(Number(floor), direction, 25);
    elevator.request(req);
};
fs.writeFileSync('output.txt', '');
question();
