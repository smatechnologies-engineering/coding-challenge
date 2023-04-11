using ElevatorApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ElevatorUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddFloorRequest()
        {
            // Arrange
            // Create a new ElevatorController instance and add some floor requests

            // Act
            // Call the Run method on the controller

            // Assert
            // Verify that the elevator stops at each requested floor
            var controller = new ElevatorController();
            controller.AddFloorRequest(5, ElevatorDirection.Up);
            controller.AddFloorRequest(2, ElevatorDirection.Down);
            controller.AddFloorRequest(7, ElevatorDirection.Up);
            controller.Run();
            Assert.IsTrue(controller._visitedFloors.Contains(5));
            Assert.IsTrue(controller._visitedFloors.Contains(2));
            Assert.IsTrue(controller._visitedFloors.Contains(7));
        }

        [TestMethod]
        public void TestAddInsideRequest()
        {
            // Arrange
            // Create a new ElevatorController instance and add some inside requests

            // Act
            // Call the Run method on the controller

            // Assert
            // Verify that the elevator stops at each requested floor
            var controller = new ElevatorController();
            controller.AddInsideRequest(5);
            controller.AddInsideRequest(2);
            controller.AddInsideRequest(7);
            controller.Run();
            Assert.IsTrue(controller._visitedFloors.Contains(5));
            Assert.IsTrue(controller._visitedFloors.Contains(2));
            Assert.IsTrue(controller._visitedFloors.Contains(7));
        }

       


    }
}
