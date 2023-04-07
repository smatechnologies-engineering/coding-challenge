using Elevator.Domain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elevator.Tests
{
    [TestFixture]
    public class TestRunProgram
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

        // Invalid Tests
        [TestCase("-1", true, nameof(Floor))]
        [TestCase("11", true, nameof(Floor))]
        [TestCase("u", true, nameof(Floor))]
        [TestCase("U", true, nameof(Floor))]
        [TestCase("10 q", true, nameof(Direction))]
        [TestCase("11 u", true, nameof(Floor))]
        [TestCase("-2 u", true, nameof(Floor))]
        [TestCase("10u", true, nameof(Floor))]
        [TestCase("10u 30", true, nameof(Floor))]
        [TestCase("3  party", true, nameof(Direction))]
        // Valid internal button press
        [TestCase("q", false, null)]
        [TestCase("1", false, null)]
        [TestCase("10", false, null)]
        [TestCase("10", false, null)]
        // Valid external button press
        [TestCase("10 d", false, null)]
        [TestCase("10 D", false, null)]
        [TestCase("10 u", false, null)]
        [TestCase("10 U", false, null)]
        [TestCase("5 U", false, null)]
        [TestCase("5 D", false, null)]
        [TestCase(" 5 D ", false, null)]
        public void TestProcessInput(string input, bool expectedError, string errorMessage)
        {
            var program = new RunProgram();
            var result = program.ProcessInput(input, Floors);
            Assert.AreEqual(expectedError, result.HasError);
            Assert.AreEqual(errorMessage, result.ErrorMessage);
        }

        [Test]
        public void TestFloorExternalButtonPress_5D()
        {
            var program = new RunProgram();
            var result = program.ProcessInput("5 d", Floors);
            Assert.IsFalse(result.HasError);
            Assert.True(Floors[5].DownButtonPressed);
        }

        [Test]
        public void TestFloorExternalButtonPress_5U()
        {
            var program = new RunProgram();
            var result = program.ProcessInput("5 u", Floors);
            Assert.IsFalse(result.HasError);
            Assert.True(Floors[5].UpButtonPressed);
        }

        [Test]
        public void TestFloorInternalButtonPress_5()
        {
            var program = new RunProgram();
            var result = program.ProcessInput("5", Floors);
            Assert.IsFalse(result.HasError);
            Assert.True(Floors[5].InternalButtonPress);
        }
    }
}
