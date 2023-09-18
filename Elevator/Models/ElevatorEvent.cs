using System;

namespace Elevator.Models
{
  public class ElevatorEvent
  {
    public ElevatorEvent(
      int FloorRequested
      // bool DirectionOfElevatorUp,
      // bool DirectionOfEventUp,
      // bool FloorRequestedBool,
      // bool FloorOn,
      // bool FloorPassedBool,
      // bool FloorStopped,
      // bool CompletedEvent
      )
    {
      this.Now = (DateTimeOffset)DateTime.UtcNow;
      this.FloorRequested = FloorRequested;

      // this.DirectionOfElevatorUp = DirectionOfElevatorUp;
    }
    //  Timestamp (Date and time including seconds) - use for timing  // using System;
    public DateTimeOffset Now { get; set; }

    public int FloorRequested { get; set; }

    public bool DirectionOfElevatorUp { get; set; }
    // bool used to determine if elevator will hit this floor when going up if true

    public bool DirectionOfEventUp { get; set; }
    // bool used when floor is Requested
    public bool FloorRequestedBool { get; set; }
    // int is floor elevator has moved to
    public int FloorOn { get; set; }
    // FloorPassedBool bool used when floor is passed
    public bool FloorPassedBool { get; set; }
    //  bool used when the floor is stoppedon
    public bool FloorStopped { get; set; }
    //  bool event is done
    public bool CompletedEvent { get; set; }
  }
}