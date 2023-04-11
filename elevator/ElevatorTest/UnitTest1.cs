using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ElevatorApp;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ElevatorTest
{
    [TestClass]
    public class UnitTest1
    {
        [Test]
        public void AddFloorRequest_ShouldAddRequest()
        {
            // Arrange
            var controller = new ElevatorController();

            // Act
            controller.AddFloorRequest(5, ElevatorDirection.Up);

            // Assert
            Assert.That(controller._floorRequests.Count, Is.EqualTo(1));
            Assert.That(controller._floorRequests[0].FloorNumber, Is.EqualTo(5));
            Assert.That(controller._floorRequests[0].Direction, Is.EqualTo(ElevatorDirection.Up));
        }
    }
}
