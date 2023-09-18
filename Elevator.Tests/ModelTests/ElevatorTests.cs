using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elevator.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
    public void AddFloor_AddFloorToVisit_ListOfEvents()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 2;
      List<ElevatorEvent> expectedAnswer = newElevator.AddEvent(floorToVisit);
      // Console.WriteLine(JsonConvert.SerializeObject(expectedAnswer));
      CollectionAssert.AreEqual(expectedAnswer, newElevator.EventsOrderedByFloor);
    }

    [TestMethod]
    // naming convention is MethodOrFieldName_Description_ReturnType
    public void AddFloor_AddFloorToVisitMultipleTimesAndSort_ListOfEvents()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit1 = 2;
      int floorToVisit2 = 8;
      int floorToVisit3 = 5;
      newElevator.AddEvent(floorToVisit1);
      newElevator.AddEvent(floorToVisit2);
      List<ElevatorEvent> expectedAnswer = newElevator.AddEvent(floorToVisit3);
      // List<ElevatorEvent> sample = new List<ElevatorEvent> { new ElevatorEvent(1) };
      Console.WriteLine(JsonConvert.SerializeObject(expectedAnswer));
      CollectionAssert.AreEqual(expectedAnswer, newElevator.EventsOrderedByFloor);
    }

    // [TestMethod]
    // public void AddFloor_AddFloorToVisitMultipleTimes_Void()
    // {
    //   ElevatorInBuilding newElevator = new ElevatorInBuilding();
    //   int floorToVisit = 2;
    //   int floorToVisit2 = 8;
    //   int floorToVisit3 = 5;
    //   List<int> expectedAnswer = new List<int> { 2, 8, 5 };
    //   newElevator.AddFloor(floorToVisit);
    //   newElevator.AddFloor(floorToVisit2);
    //   newElevator.AddFloor(floorToVisit3);
    //   CollectionAssert.AreEqual(newElevator.FloorsToVisit, expectedAnswer);
    // }

  }
}