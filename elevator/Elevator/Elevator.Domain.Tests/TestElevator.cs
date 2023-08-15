using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elevator.Domain.Tests
{
    [TestFixture]
    public class TestElevator
    {
        private List<Floor> Floors { get; set; }
        [SetUp]
        public void SetUp()
        {
            Floors = new List<Floor>() {
                new Floor(0),
                new Floor(1),
                new Floor(2),
                new Floor(3),
                new Floor(4),
                new Floor(5),
                new Floor(6),
                new Floor(7),
                new Floor(8),
                new Floor(9),
                new Floor(10)
            };
        }

        [Test]
        public async Task Test_DirectionChangeABottom()
        {
            var elevator = new Elevator(Floors, 0, Direction.Down);
            await elevator.EvaulateFloors();
            Assert.AreEqual(Direction.Up, elevator.Direction);
        }

        [Test]
        public async Task Test_DirectionChangeATop()
        {
            var elevator = new Elevator(Floors, 10, Direction.Up);
            await elevator.EvaulateFloors();
            Assert.AreEqual(Direction.Down, elevator.Direction);
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(10, 10)]
        public async Task Test_GoingUp(int currentFloor, int expected)
        {
            var elevator = new Elevator(Floors, currentFloor, Direction.Up);
            await elevator.EvaulateFloors();
            Assert.AreEqual(expected, elevator.CurrentFloor);
        }

        [TestCase(0, 0)]
        [TestCase(2, 1)]
        [TestCase(10, 9)]
        public async Task Test_GoingDown(int currentFloor, int expected)
        {
            var elevator = new Elevator(Floors, currentFloor, Direction.Down);
            await elevator.EvaulateFloors();
            Assert.AreEqual(expected, elevator.CurrentFloor);
        }

        [Test]
        public async Task Test_DownButtonPress_GoingDown()
        {
            Floors[3].ButtonPressed("d");
            var elevator = new Elevator(Floors, 3, Direction.Down);
            await elevator.EvaulateFloors();
            Assert.AreEqual(false, Floors[3].DownButtonPressed);
        }

        [Test]
        public async Task Test_UpButtonPress_GoingDown()
        {
            Floors[3].ButtonPressed("u");
            var elevator = new Elevator(Floors, 3, Direction.Down);
            await elevator.EvaulateFloors();
            Assert.AreEqual(true, Floors[3].UpButtonPressed);
        }

        [Test]
        public async Task Test_UpButtonPress_GoingUp()
        {
            Floors[3].ButtonPressed("u");
            var elevator = new Elevator(Floors, 3, Direction.Up);
            await elevator.EvaulateFloors();
            Assert.AreEqual(false, Floors[3].UpButtonPressed);
        }

        [Test]
        public async Task Test_DownButtonPress_GoingUp()
        {
            Floors[3].ButtonPressed("d");
            var elevator = new Elevator(Floors, 3, Direction.Up);
            await elevator.EvaulateFloors();
            Assert.AreEqual(true, Floors[3].DownButtonPressed);
        }

        [Test]
        public async Task Test_ButtonPressFromInside_GoingUp()
        {
            Floors[3].ButtonPressedFromElevator();
            var elevator = new Elevator(Floors, 3, Direction.Up);
            await elevator.EvaulateFloors();
            Assert.AreEqual(false, Floors[3].InternalButtonPress);
        }

        [Test]
        public async Task Test_ButtonPressFromInside_GoingDown()
        {
            Floors[3].ButtonPressedFromElevator();
            var elevator = new Elevator(Floors, 3, Direction.Down);
            await elevator.EvaulateFloors();
            Assert.AreEqual(false, Floors[3].InternalButtonPress);
        }


        [Test]
        public async Task Test_CheckingWhileGoingUp()
        {
            Floors[3].ButtonPressed("u");
            var elevator = new Elevator(Floors, 2, Direction.Up);

            await elevator.EvaulateFloors();
            Assert.AreEqual(true, Floors[3].UpButtonPressed);

            await elevator.EvaulateFloors();
            Assert.AreEqual(false, Floors[3].UpButtonPressed);

            await elevator.EvaulateFloors();
            Assert.AreEqual(false, Floors[3].UpButtonPressed);
        }

        [Test]
        public async Task Test_StopLongestPath()
        {
            Floors[3].ButtonPressed("u");
            var elevator = new Elevator(Floors, 4, Direction.Up);

            await elevator.Stop();
            Assert.AreEqual(false, Floors[3].UpButtonPressed);
        }
    }
}
