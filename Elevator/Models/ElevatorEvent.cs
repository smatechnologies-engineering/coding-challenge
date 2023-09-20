using System;

namespace Elevator.Models
{
  // 1-Timestamp and asynchronous floor request, every time one occurs.
  // 2-Timestamp and floor, every time elevator passes a floor.
  // 3-Timestamp and floor, every time elevator stops at a floor.

  // 3 types are floor request, stop on floor, pass a floor-check if stopped if so 4 second wait otherwise 3 second wait, just add the needed seconds to the timestamp
  public enum TypeOfEvent
  {
    FloorRequest,
    PassFloor,
    StopFloor
  }
  public class ElevatorEvent
  {
    public DateTimeOffset Now { get; set; }
    public TypeOfEvent TypeOfEvent { get; set; }
    public ElevatorEvent(TypeOfEvent type)
    {
      Now = (DateTimeOffset)DateTime.UtcNow;
      TypeOfEvent = type;
    }

  }
}