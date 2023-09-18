using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elevator.Models;
using System;
using System.Collections.Generic;

namespace Elevator.Tests
{
  [TestClass]
  public class ElevatorTests
  {
    [TestMethod]
    // naming convention is MethodOrFieldName_Description_ReturnType
    public void ElevatorConstructor_CreateInstanceOfElevator_Elevator()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      Assert.AreEqual(typeof(ElevatorInBuilding), newElevator.GetType());
    }

    //get floor to visit
    [TestMethod]
    // naming convention is MethodOrFieldName_Description_ReturnType
    public void AddFloor_AddFloorToVisit_Void()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 2;
      List<int> expectedAnswer = new List<int> { 2 };
      newElevator.AddFloor(floorToVisit);
      CollectionAssert.AreEqual(newElevator.FloorsToVisit, expectedAnswer);
    }

  }
}