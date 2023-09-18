using System.Collections.Generic;

namespace Elevator.Models
{
  public class ElevatorInBuilding
  {
    public int CurrentFloor { get; set; } = 1;
    public List<ElevatorEvent> EventsOrderedByFloor = new List<ElevatorEvent> { };

    public List<ElevatorEvent> AddEvent(int floorToVisit)
    {
      ElevatorEvent newEvent = new ElevatorEvent(floorToVisit);

      EventsOrderedByFloor.Add(newEvent);

      //sort floors in order
      EventsOrderedByFloor.Sort((x, y) => x.FloorRequested.CompareTo(y.FloorRequested));

      return EventsOrderedByFloor;
    }
  }
}