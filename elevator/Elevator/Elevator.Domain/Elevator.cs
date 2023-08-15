using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Elevator.Domain
{
    public class Elevator
    {
        private readonly int MAX_LIMIT = 20;

        private ILogging Logging;
        private List<Floor> Floors;

        public int CurrentFloor { get; private set; } = 0;
        public Direction Direction { get; set; } = Direction.Stopped;

        // TODO: When someone enters the elevator, increase this value
        // When someone presses a button from the inside, decrease this value
        // if max limit reach, dont stop at any external floor requests
        public int Occupancy { get; private set; } = 0;

        public Elevator(List<Floor> floors)
        {
            Logging = new Logging();
            Floors = floors;
        }

        public Elevator(List<Floor> floors, int currentFloor, Direction dir)
        {
            Logging = new Logging();
            Floors = floors;
            CurrentFloor = currentFloor;
            Direction = dir;
        }

        public async Task EvaulateFloors()
        {
            Logging.Log($"Checking Floors, current: {CurrentFloor}");
            var opened = await CheckFloors();
            if (Direction == Direction.Up)
            {
                if (CurrentFloor == Floors.Count - 1)
                {
                    Logging.LogWithTime("We reached the top; go down.");
                    Direction = Direction.Down;
                    return;
                }
                Logging.LogWithTime($"Passing Floor: {CurrentFloor}");
                CurrentFloor++;
            }
            if (Direction == Direction.Down)
            {
                if (CurrentFloor == 0)
                {
                    Logging.LogWithTime($"We reached the Bottom; go down.");
                    Direction = Direction.Up;
                    return;
                }
                Logging.LogWithTime($"Passing Floor: {CurrentFloor}");
                CurrentFloor--;
            }
            // task sleeps for a second based on if it was open
            await Task.Delay(3000);
        }

        private async Task<bool> CheckFloors()
        {
            if (await CheckInternalButtonPress())
            {
                return true;
            }

            if (await CheckGoingUp())
            {
                return true;
            }

            if (await CheckGoingDown())
            {
                return true;
            }
            return false;
        }


        private async Task StopElevatorForASecond()
        {
            Logging.LogWithTime($"Stopping at Floor: {CurrentFloor}");
            var currentState = this.Direction;
            this.Direction = Direction.Stopped;
            await Task.Delay(1000);
            this.Direction = currentState;
        }

        private async Task<bool> CheckGoingDown()
        {
            if (Floors.Any(f => f.Level == CurrentFloor && Direction == Direction.Down && f.DownButtonPressed))
            {
                var f = Floors.First(f => f.Level == CurrentFloor && Direction == Direction.Down && f.DownButtonPressed);
                await StopElevatorForASecond();
                f.ClearFloor();
                return true;
            }
            return false;
        }

        private async Task<bool> CheckGoingUp()
        {
            if (Floors.Any(f => f.Level == CurrentFloor && Direction == Direction.Up && f.UpButtonPressed))
            {
                var f = Floors.First(f => f.Level == CurrentFloor && Direction == Direction.Up && f.UpButtonPressed);
                await StopElevatorForASecond();
                f.ClearFloor();
                return true;
            }
            return false;
        }

        private async Task<bool> CheckInternalButtonPress()
        {
            if (Floors.Any(f => f.Level == CurrentFloor && f.InternalButtonPress))
            {
                var f = Floors.First(f => f.Level == CurrentFloor && f.InternalButtonPress);
                await StopElevatorForASecond();
                f.ClearFloor();
                return true;
            }
            return false;
        }

        public async Task Stop()
        {
            Logging.LogWithTime("Elevator Stopping, checking all the floors");

            // Everyone must get off of the elevator before it stops
            while (Floors.Any(f => f.DownButtonPressed || f.UpButtonPressed || f.InternalButtonPress))
            {
                await EvaulateFloors();
            }
            Logging.LogWithTime("Elevator Stopped");
            this.CurrentFloor = 0;
            this.Direction = Direction.Stopped;
            this.Occupancy = 0;
        }

        public async void Start(CancellationToken token)
        {
            Logging.Log("Elevator Started, Going UP!");
            Direction = Direction.Up;
            while (!token.IsCancellationRequested)
            {
                await EvaulateFloors();
            }
            Logging.Log("Elevator canceled");
        }
    }
}
