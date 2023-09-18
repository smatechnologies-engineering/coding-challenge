using System.Collections.Generic;

namespace Elevator.Models
{
  public class ElevatorInBuilding
  {
    public List<ElevatorEvent> EventsOrderedByFloor = new List<ElevatorEvent> { };

    public List<ElevatorEvent> AddEvent(int floorToVisit)
    {
      ElevatorEvent newEvent = new ElevatorEvent(floorToVisit);

      EventsOrderedByFloor.Add(newEvent);

      return EventsOrderedByFloor;
    }
  }
}