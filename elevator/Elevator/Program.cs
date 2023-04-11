using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ElevatorApp
{
    // Enumeration for elevator direction
    public enum ElevatorDirection
    {
        Up,
        Down,
        None
    }

    // Enumeration for elevator state
    public enum ElevatorState
    {
        Stopped,
        Moving
    }

    // Class to represent the elevator sensor
    public class ElevatorSensor
    {
        public int CurrentFloor { get; set; }
        public ElevatorDirection Direction { get; set; }
        public ElevatorState State { get; set; }
        public bool IsOverweight { get; set; }

        public ElevatorSensor()
        {
            // Initialize the sensor with default values
            CurrentFloor = 1;
            Direction = ElevatorDirection.None;
            State = ElevatorState.Stopped;
            IsOverweight = false;
        }
    }

    // Class to represent a floor request button
    public class FloorRequestButton
    {
        public int FloorNumber { get; private set; }
        public ElevatorDirection Direction { get; private set; }

        public FloorRequestButton(int floorNumber, ElevatorDirection direction)
        {
            FloorNumber = floorNumber;
            Direction = direction;
        }
    }

    // Class to represent the elevator controller
    public class ElevatorController
    {
        public readonly ElevatorSensor _sensor;
        public readonly List<FloorRequestButton> _floorRequests;
        public readonly List<int> _insideRequests;
        public readonly HashSet<int> _visitedFloors;
        public readonly string _logFilePath;
        

        public ElevatorController()
        {
            // Initialize the elevator controller with default values
            _sensor = new ElevatorSensor();
            _floorRequests = new List<FloorRequestButton>();
            _insideRequests = new List<int>();
            _visitedFloors = new HashSet<int>();
            _logFilePath = @".\elevator.txt";
        }

        public void AddFloorRequest(int floorNumber, ElevatorDirection direction)
        {
            // Add a floor request to the list
            _floorRequests.Add(new FloorRequestButton(floorNumber, direction));
            Log($"[{DateTime.Now}] Floor {floorNumber} {direction} request added.");
        }

        public void AddInsideRequest(int floorNumber)
        {
            // Add an inside request to the list
            _insideRequests.Add(floorNumber);
            Log($"[{DateTime.Now}] Inside request for floor {floorNumber} added.");
        }

        public void Run()
        {
            // Start the elevator and process requests until there are no more requests
            while (_floorRequests.Any() || _insideRequests.Any())
            {
                if (_sensor.IsOverweight)
                {
                    // If the elevator is overweight, stop only at floors that were selected from inside the elevator
                    if (_insideRequests.Any())
                    {
                        var nextFloor = GetNextInsideRequestFloor();
                        MoveElevatorToFloor(nextFloor);
                    }
                    else
                    {
                        // If there are no more inside requests, wait until weight limit is no longer exceeded
                        Log($"[{DateTime.Now}] Waiting for passengers to exit (overweight).");
                        Wait(5000);
                    }
                }
                else
                {
                    // If elevator is not overweight, process any floor requests
                    if (_floorRequests.Any())
                    {
                        var nextFloor = GetNextRequestedFloor();
                        MoveElevatorToFloor(nextFloor);
                    }
                    else
                    {
                        // If there are no more floor requests, process any inside requests
                        if (_insideRequests.Any())
                        {
                            var nextFloor = GetNextInsideRequestFloor();
                            MoveElevatorToFloor(nextFloor);
                        }
                    }
                }
            }

            Log($"[{DateTime.Now}] All requests completed. Elevator stopped.");
        }

        private int GetNextRequestedFloor()
        {
            // Get the closest floor request in the direction of motion
            var nextRequest = _floorRequests
                .Where(fr => fr.Direction == _sensor.Direction || _sensor.Direction == ElevatorDirection.None)
                .OrderBy(fr => Math.Abs(_sensor.CurrentFloor - fr.FloorNumber))
                .FirstOrDefault();

            if (nextRequest != null)
            {
                // Remove the floor request from the list and return the next floor to visit
                _floorRequests.Remove(nextRequest);
                return nextRequest.FloorNumber;
            }
            else
            {
                // If there are no floor requests in the direction of motion, reverse the direction and try again
                _sensor.Direction = _sensor.Direction == ElevatorDirection.Up ? ElevatorDirection.Down : ElevatorDirection.Up;
                return GetNextRequestedFloor();
            }
        }

        private int GetNextInsideRequestFloor()
        {
            // Get the closest inside request in any direction
            var nextRequest = _insideRequests
                .OrderBy(i => Math.Abs(_sensor.CurrentFloor - i))
                .First();

            // Remove the inside request from the list and return the next floor to visit
            _insideRequests.Remove(nextRequest);
            return nextRequest;
        }

        public void MoveElevatorToFloor(int floorNumber)
        {
            // Update the sensor data for the next floor
            var waitTime = 3000;
            var distance = Math.Abs(_sensor.CurrentFloor - floorNumber);
            var direction = floorNumber > _sensor.CurrentFloor ? ElevatorDirection.Up : ElevatorDirection.Down;
            _sensor.State = ElevatorState.Moving;
            _sensor.Direction = direction;

            // Move the elevator to the next floor
            while (_sensor.CurrentFloor != floorNumber)
            {
                if (_floorRequests.Any(fr => fr.FloorNumber == _sensor.CurrentFloor && fr.Direction == _sensor.Direction))
                {
                    // If there is a request for this floor in the direction of motion, stop and remove the request
                    _floorRequests.RemoveAll(fr => fr.FloorNumber == _sensor.CurrentFloor && fr.Direction == _sensor.Direction);
                    _visitedFloors.Add(_sensor.CurrentFloor);
                    Log($"[{DateTime.Now}] Floor {_sensor.CurrentFloor} {direction} request serviced.");
                    Wait(1000);
                }
                else if (_sensor.IsOverweight && _insideRequests.Contains(_sensor.CurrentFloor))
                {
                    // If the elevator is overweight and there is an inside request for this floor, stop and remove the request
                    _insideRequests.Remove(_sensor.CurrentFloor);
                    _visitedFloors.Add(_sensor.CurrentFloor);
                    Log($"[{DateTime.Now}] Inside request for floor {_sensor.CurrentFloor} serviced (overweight).");
                    Wait(1000);
                }

                // Update the sensor data for the next floor
                if (_sensor.CurrentFloor < floorNumber)
                    _sensor.CurrentFloor++;
                else if (_sensor.CurrentFloor > floorNumber)
                    _sensor.CurrentFloor--;

                Log($"[{DateTime.Now}] Passed floor {_sensor.CurrentFloor}.");

                // If there are any new floor requests in the opposite direction, adjust the wait time accordingly
                var oppositeRequests = _floorRequests
                    .Where(fr => fr.FloorNumber > _sensor.CurrentFloor && _sensor.Direction == ElevatorDirection.Down ||
                                 fr.FloorNumber < _sensor.CurrentFloor && _sensor.Direction == ElevatorDirection.Up);
                if (oppositeRequests.Any())
                {
                    waitTime = Math.Max(waitTime, 5000);
                }

                //Wait(3000);
            }

            // Stop the elevator and wait for passengers to exit
            _sensor.State = ElevatorState.Stopped;
            _visitedFloors.Add(_sensor.CurrentFloor);
            Log($"[{DateTime.Now}] Stopped at floor {_sensor.CurrentFloor}.");
            Wait(waitTime);
        }

        private void Wait(int milliseconds)
        {
            // Helper method to pause execution for a specified number of milliseconds
            System.Threading.Thread.Sleep(milliseconds);
        }

        private void Log(string message)
        {
            if (!File.Exists(_logFilePath))
            {
                FileStream fs = File.Create(_logFilePath);
                fs.Close();
                    // writing elevator logs to newly created file
                    using (var writer = File.AppendText(_logFilePath))
                    {
                        writer.WriteLine(message);
                    }
                
            }
            else
            {
                // writing elevator logs to newly created file
                using (var writer = File.AppendText(_logFilePath))
                {
                    writer.WriteLine(message);
                }
            }
            
           
            
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of the elevator controller
            var elevator = new ElevatorController();

            if (File.Exists(elevator._logFilePath))
            {
                // Clear the content of the file
                File.Delete(elevator._logFilePath);
                
            }
            

            // Process user input until the "Q" key is pressed
            while (true)
            {
                Console.Write("Enter floor request (e.g. 5U, 8D) or inside request (e.g. 2) and Q to end: ");
                var input = Console.ReadLine().ToUpper();
                if (input == "Q")
                    break;

                if (input.Length == 1 && int.TryParse(input, out int insideRequest))
                {
                    elevator.AddInsideRequest(insideRequest);
                }
                else if (input.Length == 2 && int.TryParse(input[0].ToString(), out int floorNumber))
                {
                    var direction = input[1] == 'U' ? ElevatorDirection.Up :
                                    input[1] == 'D' ? ElevatorDirection.Down :
                                    ElevatorDirection.None;
                    if (direction != ElevatorDirection.None)
                        elevator.AddFloorRequest(floorNumber, direction);
                }
                // Run the elevator and display the list of visited floors
                 elevator.Run();
                
            }
            Console.WriteLine("Visited floors: " + string.Join(", ", elevator._visitedFloors));
            Console.ReadLine();

        }
    }
}
