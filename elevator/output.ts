// to write to new files for output 
import * as fs from 'fs'

// to add EOL character to file output
import * as os from 'os';


class Output {
  outputFileName: string;

  constructor(filename: string) {
    this.outputFileName = filename;
  }

  output(txt:string) {
    const time = new Date();
    const hours = time.getHours();
    const minutes = time.getMinutes();
    const seconds = time.getSeconds();
    const outputString = `${hours}:${minutes}:${seconds} - ${txt}`;
    fs.appendFileSync(this.outputFileName, outputString + os.EOL);
  }

}

export default Output;