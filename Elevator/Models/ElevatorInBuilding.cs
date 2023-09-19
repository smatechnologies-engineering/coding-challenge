using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public enum Direction
{
  Up, //0
  Down, //1
  Idle //2
}

namespace Elevator.Models
{
  public class ElevatorInBuilding
  {
    private int currentFloor;
    public Direction direction;
    public List<int> floorRequests;

    public ElevatorInBuilding()
    {
      currentFloor = 1;
      direction = Direction.Idle;
      floorRequests = new List<int>();
    }

    public void RequestFloor(int floor)
    {
      Console.WriteLine(3030);
      if (floor == currentFloor)
      {
        return;
      }
      if (direction == Direction.Idle)
      {
        Console.WriteLine(3737);
        direction = (floor > currentFloor) ? Direction.Up : Direction.Down;
      }

      Console.WriteLine(4141);
      floorRequests.Add(floor);
      Run();
    }

    // Run the elevator, note on request floor we set the direction 
    public void Run()
    {
      while (floorRequests.Count > 0)
      {
        int nextFloor = GetNextFloor();
        Console.WriteLine(4848);
        MoveToFloor(nextFloor);
      }
      direction = Direction.Idle;
    }

    private int GetNextFloor()
    {
      if (direction == Direction.Up)
      {
        return floorRequests.Where(someFloor => someFloor > currentFloor).DefaultIfEmpty(floorRequests.Max()).Min();
      }
      else if (direction == Direction.Down)
      {
        return floorRequests.Where(someFloor => someFloor < currentFloor).DefaultIfEmpty(floorRequests.Min()).Max();
      }
      return currentFloor;
    }

    private void MoveToFloor(int targetFloor)
    {
      if (currentFloor < targetFloor)
      {
        direction = Direction.Up;
        while (currentFloor < targetFloor)
        {
          // wait 3 seconds for elevator to move
          Thread.Sleep(3000);
          currentFloor++;
          Console.WriteLine($"Arrived at Floor {currentFloor}");
        }
      }
      else if (currentFloor > targetFloor)
      {
        direction = Direction.Down;
        while (currentFloor > targetFloor)
        {
          Thread.Sleep(3000);
          currentFloor--;
          Console.WriteLine($"Arrived at Floor {currentFloor}");
        }
      }
      Console.WriteLine($"Elevator stopped at Floor {currentFloor}");
      floorRequests.Remove(currentFloor);
      Thread.Sleep(1000);
    }

  }
}