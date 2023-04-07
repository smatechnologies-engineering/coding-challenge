using System;

namespace Elevator.Domain
{
    public class Logging : ILogging
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWithTime(string message)
        {
            Console.WriteLine($"{DateTime.Now} - {message}");
        }
    }
}
