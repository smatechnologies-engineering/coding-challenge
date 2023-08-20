using NLog;

namespace Elevator
{
    public interface ICommandProcessor
    {
        public Task RunInputLoopAsync(ElevatorSystem elevatorSystem, int numFloors);

        public Task<bool> ProcessCommandAsync(string command, ElevatorSystem elevatorSystem);
    }

    public class CommandProcessor : ICommandProcessor
    {
        private readonly ILogger Logger;

        public CommandProcessor(ILogger logger)
        {
            Logger = logger;
        }

        public async Task RunInputLoopAsync(ElevatorSystem elevatorSystem, int numFloors)
        {
            ShowHelp(numFloors);

            while (elevatorSystem.Status == ElevatorSystem.ElevatorSystemStatus.Running)
            {
                string command = Console.ReadLine();

                var valid = await ProcessCommandAsync(command, elevatorSystem);

                if (!valid)
                {
                    Logger.Info($"Invalid command: \"{command}\"");

                    ShowHelp(numFloors);
                }
                else
                {
                    Logger.Info($"Valid command: \"{command}\"");
                }
            }
        }

        private void ShowHelp(int numFloors)
        {
            Console.WriteLine("Invalid command.  Valid commands are:");
            Console.WriteLine("'#U' - Up button pressed on floor #.    ex '5U'");
            Console.WriteLine("'#D' - Down button pressed on floor #.  ex '5D'");
            Console.WriteLine("'#'  - # button pressed on elevator.    ex '5'");
            Console.WriteLine($"Valid floor numbers are 1 to {numFloors}");
            Console.WriteLine("'W+' - set the overweight state to true.");
            Console.WriteLine("'W-' - set the overweight state to false");
            Console.WriteLine("'Q'  - Stop recieving input and quit the application after doing all scheduled stops.");
        }

        public async Task<bool> ProcessCommandAsync(string command, ElevatorSystem elevatorSystem)
        {
            if (command == null)
            {
                return false;
            }

            command = command.Trim().ToLower();
            if (command == "q")
            {
                elevatorSystem.Status = ElevatorSystem.ElevatorSystemStatus.ShuttingDown;
                return true;
            }

            if (command.StartsWith('w'))
            {
                command = command.Substring(1).Trim();

                if (command == "+")
                {
                    elevatorSystem.Elevators[0].SensorOverWeight = true;
                    return true;
                }
                else if (command == "-")
                {
                    elevatorSystem.Elevators[0].SensorOverWeight = false;
                    return true;
                }
                return false;
            }


            int floor;

            //up button pressed:  ex "5U", "15u", "25 u"
            if (command.EndsWith('u'))
            {
                command = command.Substring(0, command.Length - 1).Trim();

                if (int.TryParse(command, out floor))
                {
                    if ((0 < floor) && (floor <= elevatorSystem.NumFloors))
                    {
                        elevatorSystem.FloorStates[floor].Up = true;
                        return true;
                    }
                }

                return false;
            }

            //Down button pressed:  ex "5D", "15D", "25 D"
            if (command.EndsWith('d'))
            {
                command = command.Substring(0, command.Length - 1).Trim();

                if (int.TryParse(command, out floor))
                {
                    if ((0 < floor) && (floor <= elevatorSystem.NumFloors))
                    {
                        elevatorSystem.FloorStates[floor].Down = true;
                        return true;
                    }
                }

                return false;
            }

            //Elevator button pressed:  ex "5", "15", "25"
            if (int.TryParse(command, out floor))
            {
                if ((0 < floor) && (floor <= elevatorSystem.NumFloors))
                {
                    elevatorSystem.Elevators[0].Stops[floor] = true;
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
