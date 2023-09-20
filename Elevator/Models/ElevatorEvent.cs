using System;

namespace Elevator.Models
{
  // Timestamp and asynchronous floor request, every time one occurs.
  // Timestamp and floor, every time elevator passes a floor.
  // Timestamp and floor, every time elevator stops at a floor.
  public class ElevatorEvent

  // 3 types are floor request, stop on floor, pass a floor-check if stopped if so 4 second wait otherwise 3 second wait, just add the needed seconds to the timestamp
  {
    public DateTimeOffset Now { get; set; }
    public ElevatorEvent(string type)
    {
      this.Now = (DateTimeOffset)DateTime.UtcNow;
      this.FloorRequested = FloorRequested;
    }
    
  }
}