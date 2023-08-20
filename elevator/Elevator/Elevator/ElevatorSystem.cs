namespace Elevator
{
    public class FloorState
    {
        public bool Up { get; set; } = false;
        public bool Down { get; set; } = false;
    }

    public class ElevatorSystem
    {
         public enum ElevatorSystemStatus
         {
             Running,
             ShuttingDown,
             ShutDown,
         }

        public ICommandProcessor CommandProcessor { get; private set; }

        public IElevatorControl ElevatorControl { get; private set; }

        public ElevatorSystemStatus Status { get; set; } = ElevatorSystemStatus.Running;

        public int NumFloors { get; set; }

        public Dictionary<int, FloorState> FloorStates { get; set; } = new Dictionary<int, FloorState>();

        //Storing elevators as a list even though everything only uses one as defensive coding
        //Changing a datamodel from 1:1 to many:1 has been painful enough in the past that when
        //it's an obvious future requirement I prefer to build it in from the start.
        public List<Elevator> Elevators { get; set; } = new List<Elevator>();

        public ElevatorSystem(int numFloors, ElevatorSystemStatus status, ICommandProcessor commandProcessor, IElevatorControl elevatorControl)
        {
            CommandProcessor = commandProcessor;
            ElevatorControl = elevatorControl;

            NumFloors = numFloors;
            Status = status;
            Elevators.Add(new Elevator(numFloors));
            for (int i = 0; i < numFloors; i++)
            {
                FloorStates.Add(i + 1, new FloorState());
            }
        }

        public async Task RunElevatorSystem()
        {
            //block on the elevator operation not the input loop.  Both need to run concurrently, but we want the input processing
            //to stop immediately, but the app to continue until all stops are made.
            Task.Run(() => CommandProcessor.RunInputLoopAsync(this, NumFloors));
            await ElevatorControl.RunElevatorSystemControlLoop(this, 0);
        }
    }
}