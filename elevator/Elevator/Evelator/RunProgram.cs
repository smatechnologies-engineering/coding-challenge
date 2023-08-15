using Elevator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elevator
{
    public class RunProgram
    {
        private ILogging logging;
        private List<Floor> floors { get; set; }
        public RunProgram()
        {
            floors = CreateFloors();
            logging = new Logging();
            logging.Log("Enter a floor command: ie. (5 u = 5th floor, up button pressed)");
            logging.Log("Enter a elevator command: ie. (5 = stop at 5th floor)");
        }

        public async Task Run()
        {
            var input = "";
            var elevator = new Elevator.Domain.Elevator(floors);
            CancellationTokenSource source = new CancellationTokenSource();
            await Task.Run(() => elevator.Start(source.Token));
            while (input.ToLowerInvariant() != "q")
            {
                input = Console.ReadLine().Trim();

                ProcessInput(input, floors);
            }
            source.Cancel();
            await elevator.Stop();
            Console.ReadLine();
        }

        public TaskResult ProcessInput(string input, List<Floor> floors)
        {
            input = input.Trim();
            if (input.Equals("q"))
            {
                return TaskResult.Success();
            }
            if (!input.Contains(" "))
            {
                if (IsFloorValid(input))
                {
                    var buttonPressedInside = floors.First(f => f.Level == int.Parse(input));
                    buttonPressedInside.ButtonPressedFromElevator();
                }
                else
                {
                    logging.Log("Invalid Floor: 0 - 10 plz.");
                    return TaskResult.Error(nameof(Floor));
                }
            }
            else if (input.Contains(" "))
            {
                try
                {
                    var temp = input.Split(" ");
                    var floor = temp[0].Trim();
                    var direction = temp[1].Trim();
                    var result = IsValidInput(floor, direction);
                    if (!result.HasError)
                    {
                        var selectedFloor = floors.First(f => f.Level == int.Parse(floor));
                        selectedFloor.ButtonPressed(direction);
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    return TaskResult.Error("Directions unclear");
                }
            }
            else
            {
                logging.Log("Follow directions plz");
                return TaskResult.Error("Directions unclear");
            }
            return TaskResult.Success();
        }

        private TaskResult IsValidInput(string floor, string direction)
        {
            if (!IsFloorValid(floor))
            {
                logging.Log("Invalid Floor: 0 - 10 plz.");
                return TaskResult.Error(nameof(Floor));
            }
            if (!ProcessDirection(direction))
            {
                logging.Log("Invalid Direction: <U> or <D> plz.");
                return TaskResult.Error(nameof(Direction));
            }
            return TaskResult.Success();
        }

        private bool IsFloorValid(string floor)
        {
            return int.TryParse(floor, out var f) && (f >= 0 && f < 11);
        }
        private bool ProcessDirection(string direction)
        {
            return direction.ToLowerInvariant().Equals("u") || direction.ToLowerInvariant().Equals("d");
        }

        private List<Floor> CreateFloors()
        {
            return new List<Floor>() {
                new Floor(0),
                new Floor(1),
                new Floor(2),
                new Floor(3),
                new Floor(4),
                new Floor(5),
                new Floor(6),
                new Floor(7),
                new Floor(8),
                new Floor(9),
                new Floor(10)
            };
        }
    }
}
