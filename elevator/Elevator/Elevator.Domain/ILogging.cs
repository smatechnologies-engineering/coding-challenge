namespace Elevator.Domain
{
    public interface ILogging
    {
        void Log(string message);
        void LogWithTime(string message);
    }
}