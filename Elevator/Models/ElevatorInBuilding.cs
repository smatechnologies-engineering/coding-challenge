using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;

namespace Elevator.Models
{
  public enum Direction
  {
    Up, //0
    Down, //1
    Idle //2
  }
  public class ElevatorInBuilding
  {
    public int currentFloor;
    public Direction direction;
    public List<int> floorRequests = new List<int>();
    public int nextFloorToVisit;
    public List<ElevatorEvent> events = new List<ElevatorEvent>();

    public ElevatorInBuilding()
    {
      currentFloor = 1;
      direction = Direction.Idle;
      // floorRequests = new List<int>();
    }

    public void RequestFloor(int floor)
    {
      if (floor == currentFloor)
      {
        return;
      }
      if (direction == Direction.Idle)
      {
        direction = (floor > currentFloor) ? Direction.Up : Direction.Down;
        Console.WriteLine($"Direction was idle, now being set to {direction}");
      }

      floorRequests.Add(floor);
      ElevatorEvent floorRequest = new ElevatorEvent(TypeOfEvent.FloorRequest);
      events.Add(floorRequest);
      //need to sort the floors here however if one floor away hold on for 3 seconds before doing the sort since the elevator cannot stop there on time
      if (Math.Abs(floor - currentFloor) == 1)
      {
        Thread.Sleep(3000);
      }
      floorRequests.Sort();
    }

    // Run the elevator, note on request floor we set the direction 
    public void Run()
    {
      while (floorRequests.Count > 0)
      {
        Console.WriteLine("Calculating next floor");
        int floor = GetNextFloor();
        nextFloorToVisit = floor;
        Console.WriteLine("Moving to next floor");
        Console.WriteLine(nextFloorToVisit);
        MoveToFloor(nextFloorToVisit);
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
          events.Add(new ElevatorEvent(TypeOfEvent.PassFloor));
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
          events.Add(new ElevatorEvent(TypeOfEvent.PassFloor));
        }
      }
      Console.WriteLine($"Elevator stopped at Floor {currentFloor}");
      events.Add(new ElevatorEvent(TypeOfEvent.StopFloor));
      floorRequests.Remove(currentFloor);
      Thread.Sleep(1000);
    }

  }
}