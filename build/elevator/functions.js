import * as fs from 'fs';
import * as os from 'os';
export const output = (txt) => {
    fs.appendFileSync('output.txt', txt + os.EOL);
};
export const sum = (arr) => {
    let sum = 0;
    sum = arr.reduce((a, b) => a + b);
    return sum;
};
