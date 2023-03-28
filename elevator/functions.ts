import * as fs from 'fs';
import * as os from 'os';


export const output = (txt: string) => {
  fs.appendFileSync('output.txt', txt + os.EOL);
}

export const sum = (arr: number[]) => {
  let sum = 0;
  sum = arr.reduce((a,b) => a + b);
  return sum;
}
