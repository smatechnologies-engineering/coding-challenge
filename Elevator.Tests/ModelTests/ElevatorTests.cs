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
      CollectionAssert.AreEqual(expectedAnswer, newElevator.floorRequests);
    }

    // CheckThatThereIsDelayToAddOneFloorAway idea do 3,4,2 check the current floor or next floor
    // [TestMethod]
    // public void Run_CheckThatThereIsDelayToAddOneFloorAway_Void()
    // {
    //   ElevatorInBuilding newElevator = new ElevatorInBuilding();
    //   int floorToVisit = 3;
    //   int floorToVisit2 = 4;
    //   int floorToVisit3 = 2;
    //   newElevator.RequestFloor(floorToVisit);
    //   //need to Run
    //   newElevator.Run();
    //   Thread.Sleep(2000);
    //   newElevator.RequestFloor(floorToVisit2);
    //   Thread.Sleep(2000);
    //   int nextFloorYouGoTo = newElevator.nextFloorToVisit;
    //   Console.WriteLine(JsonConvert.SerializeObject(nextFloorYouGoTo));
    //   Assert.AreEqual(nextFloorYouGoTo, floorToVisit);
    //   newElevator.RequestFloor(floorToVisit3);
    //   List<int> expectedAnswer = new List<int> { 3, 4, 1 };
    //   Console.WriteLine(JsonConvert.SerializeObject(newElevator.floorRequests));
    //   CollectionAssert.AreEqual(expectedAnswer, newElevator.floorRequests);
    // }



  }
}