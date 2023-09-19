using System.Collections.Generic;
using System;
using System.Threading;

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
    private Direction direction;
    public List<int> floorRequests;

    public ElevatorInBuilding()
    {
      currentFloor = 1;
      direction = Direction.Idle;
      floorRequests = new List<int>();
    }

    public void RequestFloor(int floor)
    {
      if(floor == currentFloor)
      {
        return;
      }
      if(direction == Direction.Idle)
      {
        direction = (floor > currentFloor) ? Direction.Up : Direction.Down;
      }

      floorRequests.Add(floor);
    }


  }
}