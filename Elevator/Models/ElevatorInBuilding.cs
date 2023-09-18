using System.Collections.Generic;

namespace Elevator.Models
{
  public class ElevatorInBuilding
  {
    public List<int> FloorsToVisit = new List<int> { };

    public void AddFloor(int floor)
    {
      FloorsToVisit.Add(floor);
    }
  }
}