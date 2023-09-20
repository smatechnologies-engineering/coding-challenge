using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elevator.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;

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
    public void AddFloor_AddFloorToVisit_ListOfInt()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 2;
      newElevator.RequestFloor(floorToVisit);
      List<int> expectedAnswer = new List<int> { 2 };
      Console.WriteLine(JsonConvert.SerializeObject(newElevator.floorRequests));
      CollectionAssert.AreEqual(expectedAnswer, newElevator.floorRequests);
    }

    [TestMethod]
    public void RequestFloor_AddMultipleFloorsInSortedManner_Void()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 5;
      int floorToVisit2 = 3;
      newElevator.RequestFloor(floorToVisit);
      newElevator.RequestFloor(floorToVisit2);
      List<int> expectedAnswer = new List<int> { 3, 5 };
      Console.WriteLine(JsonConvert.SerializeObject(newElevator.floorRequests));
      Assert.AreEqual(newElevator.direction, Direction.Up);
      CollectionAssert.AreEqual(expectedAnswer, newElevator.floorRequests);
    }

    [TestMethod]
    public void RequestFloor_DirectionIsUp_Void()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 5;
      int floorToVisit2 = 3;
      newElevator.RequestFloor(floorToVisit);
      newElevator.RequestFloor(floorToVisit2);
      Assert.AreEqual(newElevator.direction, Direction.Up);
    }

    [TestMethod]
    public void Run_NextFloorToVisitIsCalculated_Void()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 5;
      newElevator.RequestFloor(floorToVisit);
      newElevator.Run();
      Assert.AreEqual(newElevator.nextFloorToVisit, floorToVisit);
    }

    [TestMethod]
    public void Run_FloorRequestsAreRemovedAfterVisited_Void()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 5;
      int floorToVisit2 = 3;
      newElevator.RequestFloor(floorToVisit);
      newElevator.RequestFloor(floorToVisit2);
      newElevator.Run();
      List<int> blankList = new List<int>() { };
      CollectionAssert.AreEqual(newElevator.floorRequests, blankList);
    }

    [TestMethod]
    public void Run_AddEventsToElevator_Void()
    {
      ElevatorInBuilding newElevator = new ElevatorInBuilding();
      int floorToVisit = 3;
      newElevator.RequestFloor(floorToVisit);
      newElevator.Run();
      ElevatorEvent newEvent1 = new ElevatorEvent(TypeOfEvent.FloorRequest);
      ElevatorEvent newEvent2 = new ElevatorEvent(TypeOfEvent.PassFloor); // 1
      ElevatorEvent newEvent3 = new ElevatorEvent(TypeOfEvent.PassFloor); // 2
      ElevatorEvent newEvent4 = new ElevatorEvent(TypeOfEvent.StopFloor);
      List<ElevatorEvent> eventList = new List<ElevatorEvent> { newEvent1, newEvent2, newEvent3, newEvent4 };
      Assert.AreEqual(newElevator.events.Count, eventList.Count);
    }
  }
}