using Elevator;
using NLog;
using static Elevator.Elevator;
using static Elevator.ElevatorSystem;

namespace ElevatorTests
{
    public class Tests
    {
        private ElevatorSystem elevatorSystem;
        private CommandProcessor commandProcessor;
        private ElevatorControl elevatorControl;

        public Tests()
        {
            commandProcessor = new CommandProcessor(LogManager.GetLogger(typeof(CommandProcessor).FullName));
            elevatorControl = new ElevatorControl(LogManager.GetLogger(typeof(ElevatorControl).FullName));
            elevatorSystem = new ElevatorSystem(10, ElevatorSystemStatus.Running, commandProcessor, elevatorControl);
        }

        [Theory]
        [InlineData(new object[] { null, false })]
        [InlineData(new object[] { "", false })]
        [InlineData(new object[] { " ", false})]
        [InlineData(new object[] { "adsf", false })]
        [InlineData(new object[] { "11", false })]
        [InlineData(new object[] { "0", false })]
        [InlineData(new object[] { "-1", false })]
        [InlineData(new object[] { "Q", true })]
        [InlineData(new object[] { "5", true })]
        [InlineData(new object[] { " 5", true })]
        [InlineData(new object[] { "5 ", true })]
        [InlineData(new object[] { " 5 ", true })]
        [InlineData(new object[] { "5U", true })]
        [InlineData(new object[] { "5 u", true })]
        [InlineData(new object[] { "5u", true })]
        [InlineData(new object[] { " 5 u", true })]
        [InlineData(new object[] { "5u ", true })]
        [InlineData(new object[] { "5D", true })]
        [InlineData(new object[] { "5 d", true })]
        [InlineData(new object[] { "5d", true })]
        [InlineData(new object[] { " 5 d", true })]
        [InlineData(new object[] { "5d ", true })]
        [InlineData(new object[] { "W+", true })]
        [InlineData(new object[] { "w-", true })]
        [InlineData(new object[] { "ww-", false })]
        [InlineData(new object[] { "W++", false })]
        public void TestCommandValidation(string command, bool valid)
        {
            var success = commandProcessor.ProcessCommandAsync(command, elevatorSystem);
            Assert.Equal(valid, success.Result);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "5U" }, new int[] { 5 }, new int[] { }, new int[] { }, ElevatorSystemStatus.Running })]
        [InlineData(new object[] { new string[] { "Q" }, new int[] { }, new int[] { }, new int[] { }, ElevatorSystemStatus.ShuttingDown })]
        [InlineData(new object[] { new string[] { "5U", "Q" }, new int[] { 5 }, new int[] { }, new int[] { }, ElevatorSystemStatus.ShuttingDown })]
        [InlineData(new object[] { new string[] { "5U", "5U" }, new int[] { 5 }, new int[] { }, new int[] { }, ElevatorSystemStatus.Running })]
        [InlineData(new object[] { new string[] { "5U", "6U" }, new int[] { 5, 6 }, new int[] { }, new int[] { }, ElevatorSystemStatus.Running })]
        [InlineData(new object[] { new string[] { "5U", "5D" }, new int[] { 5 }, new int[] { 5 }, new int[] { }, ElevatorSystemStatus.Running })]
        [InlineData(new object[] { new string[] { "5U", "5" }, new int[] { 5 }, new int[] { }, new int[] { 5 }, ElevatorSystemStatus.Running })]
        [InlineData(new object[] { new string[] { "5U", "5", "8", "5U", "2D" }, new int[] { 5 }, new int[] { 2 }, new int[] { 5, 8 }, ElevatorSystemStatus.Running })]
        [InlineData(new object[] { new string[] { "5U", "ASDF", "ASDFU", "ASDFD", "5", "-1U", "11U", "-1D", "11D", "-5", "8", "5U", "2D" }, new int[] { 5 }, new int[] { 2 }, new int[] { 5, 8 }, ElevatorSystemStatus.Running })]
        public async void TestStateAfterCommands(string[] commands, int[] upStops, int[] downStops, int[] elevatorStops, ElevatorSystemStatus status)
        {
            await LoadCommands(commandProcessor, commands);

            for (int floor = 1; floor <= elevatorSystem.NumFloors; floor++)
            {
                Assert.Equal(upStops.Contains(floor), elevatorSystem.FloorStates[floor].Up);
                Assert.Equal(downStops.Contains(floor), elevatorSystem.FloorStates[floor].Down);

                foreach(var elevatorState in elevatorSystem.Elevators)
                {
                    Assert.Equal(elevatorStops.Contains(floor), elevatorState.Stops[floor]);
                }
            }

            Assert.Equal(status, elevatorSystem.Status);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "5U" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2D" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5D" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "5U" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2U" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2D", "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U", "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D", "5D" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D", "5U" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "5" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2D", "5" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2", "5" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2", "5" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2", "5U" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U", "5", "W+" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2", "5D", "W+" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        public async void TestNextMoveFromIdle(string[] commands, int elevatorPosition, int expectedOperationTime,
            MovingDirection expectedMovingDirection, ElevatorAction expectedElevatorAction)
        {
            await LoadCommands(commandProcessor, commands);
            elevatorSystem.Elevators[0].SensorCurrentPosition = elevatorPosition;
            elevatorSystem.Elevators[0].CurrentAction = ElevatorAction.Idle;

            int time = elevatorControl.NextMove(elevatorSystem, 0);

            Assert.Equal(expectedOperationTime, time);
            Assert.Equal(expectedMovingDirection, elevatorSystem.Elevators[0].DesiredMovingDirection);
            Assert.Equal(expectedElevatorAction, elevatorSystem.Elevators[0].CurrentAction);
        }


        [Theory]
        [InlineData(new object[] { new string[] { "5U" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2D" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5D" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "5U" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2U" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2D", "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U", "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D", "5D" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D", "5U" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "1D", "5U" }, 3, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "1D", "5U" }, 3, 3, MovingDirection.Down, ElevatorAction.Moving })]
        public async void TestNextMoveWhileMoving(string[] commands, int elevatorPosition, int expectedOperationTime,
            MovingDirection movingDirection, ElevatorAction expectedElevatorAction)
        {
            await LoadCommands(commandProcessor, commands);
            elevatorSystem.Elevators[0].SensorCurrentPosition = elevatorPosition;
            elevatorSystem.Elevators[0].SensorNextPosition = elevatorPosition + (movingDirection == MovingDirection.Up ? 1 : -1);
            elevatorSystem.Elevators[0].CurrentAction = ElevatorAction.Moving;
            elevatorSystem.Elevators[0].DesiredMovingDirection = movingDirection;
            elevatorSystem.Elevators[0].SensorMovingDirection = movingDirection;

            int time = elevatorControl.NextMove(elevatorSystem, 0);

            Assert.Equal(expectedOperationTime, time);
            Assert.Equal(movingDirection, elevatorSystem.Elevators[0].DesiredMovingDirection);
            Assert.Equal(expectedElevatorAction, elevatorSystem.Elevators[0].CurrentAction);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "4U" }, 4, MovingDirection.Up, 1, MovingDirection.Up, ElevatorAction.Open })]
        [InlineData(new object[] { new string[] { "4D" }, 4, MovingDirection.Up, 1, MovingDirection.Down, ElevatorAction.Open })]
        [InlineData(new object[] { new string[] { "4U" }, 4, MovingDirection.Down, 1, MovingDirection.Up, ElevatorAction.Open })]
        [InlineData(new object[] { new string[] { "4D" }, 4, MovingDirection.Down, 1, MovingDirection.Down, ElevatorAction.Open })]
        //NOTE:  These two scenarios are an edge case where my interpretation of the spec isn't how I'd expect a real elevator to behave.
        //The elevator would only be coming to a stop here if when it passed the previous floor the change direction request at floor 4 was the only button pressed,
        //with the second stop added while in transit.
        //In this case, because the elevator decided to stop because of the request for a stop the other direction, I would expect it to have committed to reversal
        //when it decided to make the stop, instead of making a stop and then going in the opposite direction of the people whose button press called for the stop.
        [InlineData(new object[] { new string[] { "4D", "5U" }, 4, MovingDirection.Up, 1, MovingDirection.Up, ElevatorAction.Open })]
        [InlineData(new object[] { new string[] { "4U", "3D" }, 4, MovingDirection.Down, 1, MovingDirection.Down, ElevatorAction.Open })]
        public async void TestNextMoveWhileStopping(string[] commands, int elevatorPosition, MovingDirection previousMovingDirection, int expectedOperationTime,
            MovingDirection expectedMovingDirection, ElevatorAction expectedElevatorAction)
        {
            await LoadCommands(commandProcessor, commands);
            elevatorSystem.Elevators[0].SensorCurrentPosition = elevatorPosition;
            elevatorSystem.Elevators[0].SensorNextPosition = elevatorPosition + (previousMovingDirection == MovingDirection.Up ? 1 : -1);
            elevatorSystem.Elevators[0].CurrentAction = ElevatorAction.Stopping;
            elevatorSystem.Elevators[0].DesiredMovingDirection = previousMovingDirection;
            elevatorSystem.Elevators[0].SensorMovingDirection = previousMovingDirection;

            int time = elevatorControl.NextMove(elevatorSystem, 0);

            Assert.Equal(expectedOperationTime, time);
            Assert.Equal(expectedMovingDirection, elevatorSystem.Elevators[0].DesiredMovingDirection);
            Assert.Equal(expectedElevatorAction, elevatorSystem.Elevators[0].CurrentAction);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "5U" }, 4, MovingDirection.Up, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5" }, 4, MovingDirection.Up, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "6U" }, 4, MovingDirection.Up, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "6" }, 4, MovingDirection.Up, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "6", "3" }, 4, MovingDirection.Up, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "6U", "3" }, 4, MovingDirection.Up, 3, MovingDirection.Up, ElevatorAction.Moving })]

        [InlineData(new object[] { new string[] { "3U" }, 4, MovingDirection.Down, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "3" }, 4, MovingDirection.Down, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2U" }, 4, MovingDirection.Down, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2" }, 4, MovingDirection.Down, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2", "6" }, 4, MovingDirection.Down, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U", "6" }, 4, MovingDirection.Down, 3, MovingDirection.Down, ElevatorAction.Moving })]

        [InlineData(new object[] { new string[] { "3D" }, 4, MovingDirection.Up, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "3" }, 4, MovingDirection.Up, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2U" }, 4, MovingDirection.Up, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2" }, 4, MovingDirection.Up, 3, MovingDirection.Down, ElevatorAction.Moving })]

        [InlineData(new object[] { new string[] { "5D" }, 4, MovingDirection.Down, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5" }, 4, MovingDirection.Down, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "6U" }, 4, MovingDirection.Down, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "6" }, 4, MovingDirection.Down, 3, MovingDirection.Up, ElevatorAction.Moving })]

        [InlineData(new object[] { new string[] { }, 4, MovingDirection.Up, -1, MovingDirection.None, ElevatorAction.Idle })]
        [InlineData(new object[] { new string[] { }, 4, MovingDirection.Down, -1, MovingDirection.None, ElevatorAction.Idle })]
        public async void TestNextMoveAfterOpen(string[] commands, int elevatorPosition, MovingDirection previousMovingDirection, int expectedOperationTime,
            MovingDirection expectedMovingDirection, ElevatorAction expectedElevatorAction)
        {
            await LoadCommands(commandProcessor, commands);
            elevatorSystem.Elevators[0].SensorCurrentPosition = elevatorPosition;
            elevatorSystem.Elevators[0].CurrentAction = ElevatorAction.Open;
            elevatorSystem.Elevators[0].DesiredMovingDirection = previousMovingDirection;

            int time = elevatorControl.NextMove(elevatorSystem, 0);

            Assert.Equal(expectedOperationTime, time);
            Assert.Equal(expectedMovingDirection, elevatorSystem.Elevators[0].DesiredMovingDirection);
            Assert.Equal(expectedElevatorAction, elevatorSystem.Elevators[0].CurrentAction);
        }

        [Fact]
        public void TestShutdownFromIdle() 
        {
            elevatorSystem.Status = ElevatorSystemStatus.ShuttingDown;

            int time = elevatorControl.NextMove(elevatorSystem, 0);

            Assert.Equal(ElevatorSystemStatus.ShutDown, elevatorSystem.Status);
        }

        [Theory]
        [InlineData(new object[] { new string[] { "5U" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2U" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5D" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2D" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5D" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2D" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "5U" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2U" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "5" }, 1, 3, MovingDirection.Up, ElevatorAction.Moving })]
        [InlineData(new object[] { new string[] { "2" }, 1, 3, MovingDirection.Up, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "5" }, 6, 3, MovingDirection.Down, ElevatorAction.Stopping })]
        [InlineData(new object[] { new string[] { "2" }, 6, 3, MovingDirection.Down, ElevatorAction.Moving })]
        public async void TestDontShutDownWhileNotFinished(string[] commands, int elevatorPosition, int expectedOperationTime,
            MovingDirection movingDirection, ElevatorAction expectedElevatorAction)
        {
            await LoadCommands(commandProcessor, commands);
            elevatorSystem.Status = ElevatorSystemStatus.ShuttingDown;
            elevatorSystem.Elevators[0].SensorCurrentPosition = elevatorPosition;
            elevatorSystem.Elevators[0].SensorNextPosition = elevatorPosition + (movingDirection == MovingDirection.Up ? 1 : -1);
            elevatorSystem.Elevators[0].CurrentAction = ElevatorAction.Moving;
            elevatorSystem.Elevators[0].DesiredMovingDirection = movingDirection;
            elevatorSystem.Elevators[0].SensorMovingDirection = movingDirection;

            int time = elevatorControl.NextMove(elevatorSystem, 0);

            Assert.Equal(expectedOperationTime, time);
            Assert.Equal(movingDirection, elevatorSystem.Elevators[0].DesiredMovingDirection);
            Assert.Equal(expectedElevatorAction, elevatorSystem.Elevators[0].CurrentAction);
            Assert.Equal(ElevatorSystemStatus.ShuttingDown, elevatorSystem.Status);
        }

        private async Task LoadCommands(CommandProcessor commandProcessor, IEnumerable<string> commands)
        {
            foreach (string command in commands)
            {
                await commandProcessor.ProcessCommandAsync(command, elevatorSystem);
            }
        }
    }
}