//using Microsoft.Extensions.Logging;
using NLog;
using static Elevator.Elevator;

namespace Elevator
{
    public interface IElevatorControl
    {
        public Task RunElevatorSystemControlLoop(ElevatorSystem elevatorSystem,  int elevatorNumber);
    }

    public class ElevatorControl : IElevatorControl
    {
        private readonly ILogger Logger;

        public ElevatorControl(ILogger logger)
        {
            Logger = logger;
        }

        public async Task RunElevatorSystemControlLoop(ElevatorSystem elevatorSystem, int elevatorNumber)
        {
            Elevator elevator = elevatorSystem.Elevators[elevatorNumber];

            while (elevatorSystem.Status != ElevatorSystem.ElevatorSystemStatus.ShutDown)
            {
                int operationTime = NextMove(elevatorSystem, elevatorNumber);

                if (operationTime != -1)
                {
                    Thread.Sleep(operationTime * 1000);
                    if ((elevator.CurrentAction == ElevatorAction.Moving) ||
                        (elevator.CurrentAction == ElevatorAction.Stopping))
                    {
                        //simulate updates to the sensor values
                        elevator.SensorMovingDirection = elevator.DesiredMovingDirection;
                        elevator.SensorCurrentPosition = elevator.SensorCurrentPosition + (elevator.SensorMovingDirection == MovingDirection.Up ? 1 : -1);
                        if (elevator.DesiredMovingDirection == MovingDirection.Up)
                        {
                            elevator.SensorNextPosition++;
                        }
                        else
                        {
                            elevator.SensorNextPosition--;
                        }

                        if (elevator.CurrentAction == ElevatorAction.Moving)
                        {
                            Logger.Info($"Passing floor {elevator.SensorCurrentPosition}");
                        }
                        else
                        {
                            Logger.Info($"Stopping at floor {elevator.SensorCurrentPosition}");
                        }
                    }
                    else if (elevator.CurrentAction == ElevatorAction.Open)
                    {
                        Logger.Info($"Opening at floor {elevator.SensorCurrentPosition}");
                    }
                }
                else
                {
                    //brief wait if no commands
                    Thread.Sleep(10);
                }
            }
        }

        public int NextMove(ElevatorSystem elevatorSystem, int elevatorNumber)
        {
            var elevator = elevatorSystem.Elevators[elevatorNumber];
            int timeToNextAction = -1;

            switch (elevator.CurrentAction)
            {
                case ElevatorAction.Idle:
                    bool buttonAbovePressed = ButtonAbovePressed(elevatorSystem, elevator);
                    bool buttonBelowPressed = ButtonBelowPressed(elevatorSystem, elevator);

                    if (buttonAbovePressed)
                    {
                        MoveUp(elevatorSystem, elevator);
                    }
                    else if (buttonBelowPressed)
                    {
                        MoveDown(elevatorSystem, elevator);
                    }
                    else if (elevatorSystem.Status == ElevatorSystem.ElevatorSystemStatus.ShuttingDown)
                    {
                        elevatorSystem.Status = ElevatorSystem.ElevatorSystemStatus.ShutDown;
                    }
                    break;
                case ElevatorAction.Moving:
                    if (elevator.SensorMovingDirection == MovingDirection.Up)
                    {
                        MoveUp(elevatorSystem, elevator);
                    }
                    else
                    {
                        MoveDown(elevatorSystem, elevator);
                    }
                    break;
                case ElevatorAction.Stopping:
                    //always clear the internal button
                    elevator.Stops[elevator.SensorCurrentPosition] = false;

                    if (elevator.SensorMovingDirection == MovingDirection.Up)
                    {
                        if (elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Up || ButtonAbovePressed(elevatorSystem, elevator))
                        {
                            //normal case, keep going in the same direction to serve next stop
                            elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Up = false;
                        }
                        else if (elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Down)
                        {
                            //stop was requested going the opposite direction
                            elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Down = false;
                            elevator.DesiredMovingDirection = MovingDirection.Down;
                        }
                    }
                    else
                    {
                        if (elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Down || ButtonBelowPressed(elevatorSystem, elevator))
                        {
                            elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Down = false;
                        }
                        else if (elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Up)
                        {
                            elevatorSystem.FloorStates[elevator.SensorCurrentPosition].Up = false;
                            elevator.DesiredMovingDirection = MovingDirection.Up;
                        }
                    }

                    elevator.CurrentAction = ElevatorAction.Open;
                    break;
                case ElevatorAction.Open:
                    buttonAbovePressed = ButtonAbovePressed(elevatorSystem, elevator);
                    buttonBelowPressed = ButtonBelowPressed(elevatorSystem, elevator);

                    //first two cases keep moving the way it is moving as long as there's another stop in the same direction.
                    if ((elevator.DesiredMovingDirection == MovingDirection.Up) && buttonAbovePressed)
                    {
                        MoveUp(elevatorSystem, elevator);
                    }
                    else if ((elevator.DesiredMovingDirection == MovingDirection.Down) && buttonBelowPressed)
                    {
                        MoveDown(elevatorSystem, elevator);
                    }
                    else if (buttonAbovePressed)
                    {
                        MoveUp(elevatorSystem, elevator);
                    }
                    else if (buttonBelowPressed)
                    {
                        MoveDown(elevatorSystem, elevator);
                    }
                    else
                    {
                        elevator.CurrentAction = ElevatorAction.Idle;
                        elevator.DesiredMovingDirection = MovingDirection.None;
                    }

                    break;
            }

            switch (elevator.CurrentAction)
            {
                case ElevatorAction.Idle:
                    timeToNextAction = -1;
                    break;
                case ElevatorAction.Moving:
                    timeToNextAction = 3;
                    break;
                case ElevatorAction.Stopping:
                    timeToNextAction = 3;
                    break;
                case ElevatorAction.Open:
                    timeToNextAction = 1;
                    break;
            }

            return timeToNextAction;
        }

        /// <summary>
        /// True if any button requesting a stop above the current location has been pressed, unless the elevator is overweight in which case only internal buttons are counted. 
        /// 
        /// </summary>
        /// <param name="elevatorSystem"></param>
        /// <param name="elevator"></param>
        /// <returns></returns>
        private static bool ButtonAbovePressed(ElevatorSystem elevatorSystem, Elevator elevator)
        {
            //first half checks if any buttons outside the elevator for locations above the elevator
            //were pressed unless the elevator is overweight in which case they're ignored
            //second half checks if a button above the current location in the elevator was pressed.
            return (!elevator.SensorOverWeight && elevatorSystem.FloorStates.Any(f => f.Key > elevator.SensorCurrentPosition && (f.Value.Up || f.Value.Down))) ||
                                    elevator.Stops.Any(s => s.Key > elevator.SensorCurrentPosition && s.Value == true);
        }

        private static bool ButtonBelowPressed(ElevatorSystem elevatorSystem, Elevator elevator)
        {
            return (!elevator.SensorOverWeight && elevatorSystem.FloorStates.Any(f => f.Key < elevator.SensorCurrentPosition && (f.Value.Up || f.Value.Down))) ||
                                    elevator.Stops.Any(s => s.Key < elevator.SensorCurrentPosition && s.Value == true);
        }

        private static void MoveDown(ElevatorSystem elevatorSystem, Elevator elevator)
        {
            elevator.DesiredMovingDirection = MovingDirection.Down;

            if (NextFloorDownIsAStop(elevatorSystem, elevator))
                elevator.CurrentAction = ElevatorAction.Stopping;
            else elevator.CurrentAction = ElevatorAction.Moving;
        }

        private static bool NextFloorDownIsAStop(ElevatorSystem elevatorSystem, Elevator elevator)
        {
            if (elevator.SensorOverWeight)
            {
                bool nextFloorIsOnboardStopRequest = elevator.Stops[elevator.SensorCurrentPosition - 1];
                int lastStopRequest = elevator.Stops.Where(s => s.Key < elevator.SensorCurrentPosition && s.Value).Select(s => s.Key).Min();
                bool nextButtonPressedIsLastStop = lastStopRequest == (elevator.SensorCurrentPosition - 1);
                return nextFloorIsOnboardStopRequest || nextButtonPressedIsLastStop;
            }
            else
            {
                bool nextFloorIsExernalStopRequest = elevatorSystem.FloorStates[elevator.SensorCurrentPosition - 1].Down;
                bool nextFloorIsOnboardStopRequest = elevator.Stops[elevator.SensorCurrentPosition - 1];
                int lastButtonBelowPressed = elevatorSystem.FloorStates.Where(f => f.Key < elevator.SensorCurrentPosition && (f.Value.Up || f.Value.Down)).Select(f => f.Key)
                    .Concat(elevator.Stops.Where(s => s.Key < elevator.SensorCurrentPosition && s.Value).Select(s => s.Key)).Min();
                bool nextButtonPressedIsLastStop = lastButtonBelowPressed == (elevator.SensorCurrentPosition - 1);
                return nextFloorIsExernalStopRequest || nextFloorIsOnboardStopRequest || nextButtonPressedIsLastStop;
            }
        }

        private static void MoveUp(ElevatorSystem elevatorSystem, Elevator elevator)
        {
            elevator.DesiredMovingDirection = MovingDirection.Up;


            if (NextFloorUpIsAStop(elevatorSystem, elevator))
            {
                elevator.CurrentAction = ElevatorAction.Stopping;
            }

            else elevator.CurrentAction = ElevatorAction.Moving;
        }

        private static bool NextFloorUpIsAStop(ElevatorSystem elevatorSystem, Elevator elevator)
        {
            if (elevator.SensorOverWeight)
            {
                bool nextFloorIsOnboardStopRequest = elevator.Stops[elevator.SensorCurrentPosition + 1];
                int lastButtonAbovePressed = elevator.Stops.Where(s => s.Key > elevator.SensorCurrentPosition && s.Value).Select(s => s.Key).Max();
                bool nextButtonPressedIsLastStop = lastButtonAbovePressed == (elevator.SensorCurrentPosition + 1);
                return nextFloorIsOnboardStopRequest || nextButtonPressedIsLastStop;
            }
            else
            {
                bool nextFloorIsExernalStopRequest = elevatorSystem.FloorStates[elevator.SensorCurrentPosition + 1].Up;
                bool nextFloorIsOnboardStopRequest = elevator.Stops[elevator.SensorCurrentPosition + 1];
                int lastButtonAbovePressed = elevatorSystem.FloorStates.Where(f => f.Key > elevator.SensorCurrentPosition && (f.Value.Up || f.Value.Down)).Select(f => f.Key)
                    .Concat(elevator.Stops.Where(s => s.Key > elevator.SensorCurrentPosition && s.Value).Select(s => s.Key)).Max();
                bool nextButtonPressedIsLastStop = lastButtonAbovePressed == (elevator.SensorCurrentPosition + 1);
                return nextFloorIsExernalStopRequest || nextFloorIsOnboardStopRequest || nextButtonPressedIsLastStop;
            }
        }
    }
}
