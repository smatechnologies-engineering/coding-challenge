using Elevator.Models;
// See https://aka.ms/new-console-template for more information
ElevatorInBuilding elevator = new ElevatorInBuilding();

while (true)
{
  Console.WriteLine("Input a floor request as a number and press enter or 'q' to quit then press enter, you can keep adding multiple floors just type them and press enter");
  string input = Console.ReadLine().Trim();
  if (input.Equals("q"))
  {
    break;
  }
  // check use of out
  else if (int.TryParse(input, out int floor))
  {
    Console.WriteLine("Parsing inputted value to integer");
    elevator.RequestFloor(floor);
    elevator.Run();
  }

  if (elevator.direction == Direction.Idle)
  {
    Console.WriteLine("Elevator is idle (not moving).");
    elevator.Run();
  }
}


