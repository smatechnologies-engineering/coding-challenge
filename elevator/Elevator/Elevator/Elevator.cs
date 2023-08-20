namespace Elevator
{
    public class Elevator
    {
        public enum ElevatorAction
        {
            Idle,
            Moving,
            Stopping,
            Open
        }

        public enum MovingDirection
        {
            Up, 
            Down, 
            None
        }

        public ElevatorAction CurrentAction { get; set; }

        public MovingDirection DesiredMovingDirection { get; set; }
        public MovingDirection SensorMovingDirection { get; set; }

        public int SensorCurrentPosition { get; set; }
        public int SensorNextPosition { get; set; }

        public bool SensorOverWeight { get; set; }

        public Dictionary<int, bool> Stops { get; set; } = new Dictionary<int, bool>();

        public Elevator (int numFloors)
        {
            
            for (int i = 0; i < numFloors; i++)
            {
                Stops.Add(i + 1, false);
            }

            SensorOverWeight = false;
            SensorCurrentPosition = 1;
            SensorNextPosition = 1;
            CurrentAction = ElevatorAction.Idle;
            SensorMovingDirection = MovingDirection.None;
            DesiredMovingDirection = MovingDirection.None;
        }
    }
}