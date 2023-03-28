import * as fs from 'fs';
import * as os from 'os';


export const output = (txt: string) => {
  fs.appendFileSync('output.txt', txt + os.EOL);
}
