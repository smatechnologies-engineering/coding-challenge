"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const elevator_js_1 = __importDefault(require("./elevator.js"));
const request_js_1 = __importDefault(require("./request.js"));
const fs = __importStar(require("fs"));
const readline = __importStar(require("readline"));
// Create an interface for reading from the command line
const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});
const elevator = new elevator_js_1.default(15);
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
    const req = new request_js_1.default(Number(floor), direction, 25);
    elevator.request(req);
};
fs.writeFileSync('output.txt', '');
question();
