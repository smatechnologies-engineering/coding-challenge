using CoreElevator;
using Konsole;

namespace ElevatorTests
{
    [TestClass]
    public class CoreElevatorTest
    {
        [TestMethod]
        public void TestElevatorController()
        {
            ElevatorController coreElevator = new ElevatorController(null, null);
            Assert.IsNotNull(coreElevator); 
            Assert.AreEqual(StateType.Free, coreElevator.currentState.Status);
            Assert.AreEqual(1, coreElevator.currentState.CurrentFloor);
            Assert.AreEqual(0, coreElevator.requests.TotalCurrentRequests());
        }
        [TestMethod]
        public void TestElevatorState()
        {
            ElevatorState elevatorState = new ElevatorState();
            Assert.IsNotNull(elevatorState);
            Assert.AreEqual(elevatorState.Status, StateType.Free);
            Assert.AreEqual(1, elevatorState.CurrentFloor);

        }


        [TestMethod]
        public void TestFloorRequests()
        {
            FloorRequests floorRequests = new FloorRequests(null);
            Assert.IsNotNull(floorRequests);
            Assert.AreEqual(0, floorRequests.TotalCurrentRequests());
            
        }
        [TestMethod]
        public async Task TestElevatorAddRequest()
        {
            ElevatorController coreElevator = new ElevatorController(null, null);
            Assert.IsNotNull(coreElevator);
            //Assert.AreEqual(coreElevator.currentState.Status, StateType.Free);
            Assert.AreEqual(1, coreElevator.currentState.CurrentFloor);
            Assert.AreEqual(0, coreElevator.requests.TotalCurrentRequests());

            //Thread.Sleep(1000);
            //add a request now
            coreElevator.addRequest("10U"); //not awaiting, so we know it's currently processing for tests below.
            Thread.Sleep(2000);
            Assert.AreEqual(StateType.Up, coreElevator.currentState.Status);
            Assert.AreNotEqual(1, coreElevator.currentState.CurrentFloor);
            Assert.AreEqual(1, coreElevator.requests.TotalCurrentRequests());


        }
    }
}