"use strict";
// keep track of max / min 
// be a normal object
Object.defineProperty(exports, "__esModule", { value: true });
class RequestMap {
    constructor() {
        this.map = new Map();
        this.max = -1;
        this.min = -1;
    }
    set(key, value) {
        this.map.set(key, value);
        if (this.map.size === 0) {
            this.min = key;
            this.max = key;
        }
        if (key > this.max)
            this.max = key;
        if (key < this.min || this.min === -1)
            this.min = key;
    }
    delete(key) {
        this.map.delete(key);
        if (key === this.min)
            this.setMin();
        if (key === this.max)
            this.setMax();
    }
    setMin() {
        if (!this.map.size) {
            this.min = -1;
            return;
        }
        this.min = Math.min(...Array.from(this.map.keys()));
    }
    setMax() {
        if (!this.map.size) {
            this.max = -1;
            return;
        }
        this.max = Math.max(...Array.from(this.map.keys()));
    }
}
exports.default = RequestMap;
