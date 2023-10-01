using System;

public class LogEntry
{
    private string logFormat = string.Empty;
    private string logPath = string.Empty;

    public LogEntry()
    {
        //WriteLog(logMessage);
    }

    /// <summary>
    /// Writes a log entry to a log file with hardcoded name. It is comma separated to make ingesting it easier
    /// </summary>
    /// <param name="logMessage"></param>
    // 
    //
    public void WriteLog(string logMessage, logType type = logType.FloorRequest)
    {
        logPath = System.AppContext.BaseDirectory;
        logFormat = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss") + ", " + type + ", ";
        
        try
        {
            using (StreamWriter writer = File.AppendText(logPath +
                "\\" + "logs.text"))
            {
                // Writes a string followed by a line terminator asynchronously to the stream.
                //writer.WriteLineAsync(logFormat + logMessage ); 

                writer.WriteLine(logFormat + logMessage);
            }
        }
        catch (Exception ex)
        {
            //Do not do anything 
        }
    }
    public enum logType
    {
        FloorRequest = 0,
        FloorEvent = 1
    }
}
