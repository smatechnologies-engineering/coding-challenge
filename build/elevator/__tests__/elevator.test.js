"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const elevator_js_1 = __importDefault(require("../elevator.js"));
jest.useFakeTimers();
describe('checkOn', () => {
    let elevator;
    beforeEach(() => {
        elevator = new elevator_js_1.default(15);
        elevator.externalRequestMap.set(12, { '1': [], '-1': [1] });
        elevator.currentFloor = 12;
        elevator.currentDestination = 12;
        elevator.currentDirection = 1;
    });
    afterEach(() => {
        jest.clearAllTimers();
    });
    it('should not add new people to boardingPassengers if at or above the weight limit', () => {
        elevator.currentWeight = elevator.weightLimit;
        elevator.checkOn();
        expect(elevator.boardingPassengers.length).toEqual(0);
    });
    it('should add passengers to boardingPassengers if below weightLimit', () => {
        elevator.checkOn();
        expect(elevator.boardingPassengers.length).toEqual(1);
    });
    it('should only add passengers to boardingPassengers if they are on the current floor', () => {
        elevator.externalRequestMap.set(13, { '1': [], '-1': [1] });
        elevator.checkOn();
        expect(elevator.boardingPassengers.length).toEqual(1);
    });
    it('should remove passengers from elevator.externalRequestMap when added to elevator.boardingPassengers', () => {
        elevator.checkOn();
        expect(elevator.externalRequestMap.map.size).toEqual(0);
    });
    it('should increment weight when passengers added to boardingPassengers', () => {
        elevator.checkOn();
        expect(elevator.currentWeight).toEqual(1);
    });
    it('should only add passengers traveling in same direction as the elevator currently', () => {
        elevator.externalRequestMap.map.get(12)[1].push(1);
        elevator.checkOn();
        expect(elevator.externalRequestMap.map.get(12)[1].length).toEqual(0);
        expect(elevator.externalRequestMap.map.get(12)[-1].length).toEqual(1);
    });
    it('should change direction if we reach current destination and there is a request in opposite direction', () => {
        elevator.currentFloor = 11;
        elevator.move();
        expect(elevator.currentDirection).toEqual(-1);
    });
});
describe('checkOff', () => {
    let elevator;
    beforeEach(() => {
        // Create a new Elevator instance before each test
        elevator = new elevator_js_1.default(15);
        elevator.departureRequestMap.set(15, [1]);
        elevator.currentFloor = 15;
        elevator.currentDestination = 15;
        elevator.currentDirection = 1;
        elevator.currentWeight = 12;
    });
    afterEach(() => {
        jest.clearAllTimers();
    });
    it('should only add people to departingPassengers if they are on current floor', () => {
        elevator.departureRequestMap.set(13, [1]);
        elevator.checkOff();
        expect(elevator.departingPassengers.length).toEqual(1);
    });
    it('should delete key from departureRequestMap if it has no passengers left', () => {
        elevator.checkOff();
        expect(elevator.departureRequestMap.map.size).toEqual(0);
    });
    it('should decrement weight when passenger is added to departingPassengers', () => {
        elevator.checkOff();
        expect(elevator.currentWeight).toEqual(11);
    });
});
describe('request', () => {
    let elevator;
    beforeEach(() => {
        elevator = new elevator_js_1.default(15);
    });
    it('should add request to externalRequestMap', () => {
        elevator.request({ currentFloor: 1, direction: 1, weight: 1 });
        expect(elevator.externalRequestMap.map.size).toEqual(1);
    });
});
describe('selectFloor', () => {
    let elevator;
    beforeEach(() => {
        elevator = new elevator_js_1.default(15);
    });
    it('should move passengers weight from passengerRequestQueue to departureRequestMap', () => {
        elevator.passengerRequestQueue.push(1);
        elevator.selectFloor(5);
        expect(elevator.departureRequestMap.map.size).toEqual(1);
        expect(elevator.passengerRequestQueue.length).toBe(0);
    });
});
describe('checkDestination under weight limit', () => {
    let elevator;
    beforeEach(() => {
        elevator = new elevator_js_1.default(15);
        elevator.externalRequestMap.set(15, { '1': [], '-1': [1] });
        elevator.externalRequestMap.set(14, { '1': [], '-1': [1] });
        elevator.externalRequestMap.set(13, { '1': [], '-1': [1] });
        elevator.departureRequestMap.set(12, [1]);
        elevator.departureRequestMap.set(11, [1]);
        elevator.departureRequestMap.set(10, [1]);
    });
    it('should pick the highest number floor while traveling up as the destination', () => {
        elevator.currentDirection = 1;
        elevator.currentFloor = 1;
        elevator.checkDestination();
        expect(elevator.currentDestination).toEqual(15);
    });
    it('should pick the lowest number floor while traveling down as the destination', () => {
        elevator.currentDirection = -1;
        elevator.currentDestination = -1;
        elevator.currentFloor = 15;
        elevator.checkDestination();
        expect(elevator.currentDestination).toEqual(10);
    });
    it('should change direction if destination is set opposite of current direction', () => {
        elevator.currentDestination = -1;
        elevator.currentFloor = 9;
        elevator.currentDirection = -1;
        elevator.checkDestination();
        expect(elevator.currentDirection).toEqual(1);
    });
});
describe('checkDestination over weight limit', () => {
    let elevator;
    beforeEach(() => {
        elevator = new elevator_js_1.default(15);
        elevator.externalRequestMap.set(15, { '1': [], '-1': [1] });
        elevator.externalRequestMap.set(14, { '1': [], '-1': [1] });
        elevator.externalRequestMap.set(13, { '1': [], '-1': [1] });
        elevator.departureRequestMap.set(12, [1]);
        elevator.departureRequestMap.set(11, [1]);
        elevator.departureRequestMap.set(10, [1]);
        elevator.externalRequestMap.set(9, { '1': [], '-1': [1] });
        elevator.currentWeight = elevator.weightLimit;
    });
    it('should pick the highest number in departurerequestmap while travelling up and ignore ext requests', () => {
        elevator.currentDirection = 1;
        elevator.currentFloor = 1;
        elevator.checkDestination();
        expect(elevator.currentDestination).toEqual(12);
    });
    it('should pick the lowest number from departure request map while traveling down and ignore ext requests', () => {
        elevator.currentDirection = -1;
        elevator.currentDestination = -1;
        elevator.currentFloor = 15;
        elevator.checkDestination();
        expect(elevator.currentDestination).toEqual(10);
    });
});
describe('move', () => {
    let elevator;
    beforeEach(() => {
        elevator = new elevator_js_1.default(15);
    });
    it('should stop if there are passengers getting off/on', () => {
        elevator.boardingPassengers.push(1);
        elevator.state = 'moving';
        elevator.move();
        expect(elevator.state).toEqual('stopped');
    });
    it('should increment currentdirection if we are going up', () => {
        elevator.currentDirection = 1;
        elevator.currentFloor = 11;
        elevator.departureRequestMap.set(14, [1]);
        elevator.move();
        expect(elevator.currentFloor).toEqual(12);
    });
    it('should decrement currentdirection if we are going down', () => {
        elevator.currentDirection = -1;
        elevator.currentFloor = 14;
        elevator.departureRequestMap.set(10, [1]);
        elevator.move();
        expect(elevator.currentFloor).toEqual(13);
    });
    it('should change dest to -1 when we reach current destination with no other requests', () => {
        elevator.currentDestination = 14;
        elevator.currentFloor = 14;
        elevator.departingPassengers.push(1);
        elevator.move();
        expect(elevator.state).toEqual('stopped');
        expect(elevator.currentDestination).toEqual(-1);
    });
    it('should correctly select dest when there is no destination and new request comes in', () => {
        elevator.currentDestination = -1;
        elevator.currentFloor = 14;
        elevator.currentDirection = -1;
        elevator.departureRequestMap.set(1, [1]);
        elevator.move();
        expect(elevator.currentDestination).toEqual(1);
    });
});
