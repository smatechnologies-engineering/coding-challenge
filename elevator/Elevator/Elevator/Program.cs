using NLog;

namespace Elevator
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly int numFloors = 10;

        static void Main(string[] args)
        {
            LogManager.Setup().LoadConfiguration(builder => {
                //builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
                builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(fileName: "elevator.log");
            });
            Logger.Info("\r\n");
            Logger.Info("Starting up.");

            Run();
            Logger.Info("Shutting down.");
        }

        static async void Run() 
        {
            ElevatorSystem elevatorSystem = new ElevatorSystem(numFloors, ElevatorSystem.ElevatorSystemStatus.Running, 
                new CommandProcessor(LogManager.GetLogger(typeof(CommandProcessor).FullName)), 
                new ElevatorControl(LogManager.GetLogger(typeof(ElevatorControl).FullName)));

            await elevatorSystem.RunElevatorSystem();
        }
    }
}