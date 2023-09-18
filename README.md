Build an Elevator Coding Challenge!
The Challenge
Create an application that simulates the operation of a simple elevator.

Requirements
The elevator must travel in one direction at a time until it needs to go no further (e.g. keep going until the elevator has reached the top/bottom of the building, or no stop is requested on any floor ahead).
Elevator floor request buttons can be pressed asynchronously from inside or outside the elevator while it is running.
Elevator will stop at the closest floor first, in the direction of motion, then the next closest and so on. Any floors requested while the elevator is moving should be taken into account.
Elevator will stop at all asynchronously requested floors, only if the request is made while the elevator is at least one floor away (e.g. if elevator is between 4th and 5th floor, going up, and the 5th floor is requested at that moment, elevator will not stop at the 5th floor while going up; it will stop there while going down).
When elevator arrives at a requested floor, it waits for 1 second. It takes 3 seconds to travel between consecutive floors.
A sensor tells the elevator its direction, next/current floor, state (stopped, moving) and if the elevator has reached its max weight limit.
Use the sensor data plus the asynchronous floor request button data to work the elevator.
Write meaningful unit tests that show the elevator works correctly, even if the application is not run.
Log the following to a file, to verify elevator works well:
Timestamp and asynchronous floor request, every time one occurs.
Timestamp and floor, every time elevator passes a floor.
Timestamp and floor, every time elevator stops at a floor.
Bonus Enhancement:

Enhance the application as follows: If the elevator has reached its weight limit, it should stop only at floors that were selected from inside the elevator (to let passengers out), until it is no longer at the max weight limit.
Note: For simplicity, the asynchronous request buttons can be entered by the application user via the console, by entering "5U" (request from 5th floor wanting to go Up) or "8D" (request from 8th floor wanting to go Down) or "2" (request from inside elevator wanting to stop at 2nd floor). When the user enters "Q" on the console, the application must end after visiting all floors entered before "Q".

DK ideas- use the elevator events to track what the elevator will do,

Elevator fields and methods
DirectionOfElevator
CheckDirection aka if going up see events up has value if so keep going
Events, note everytime a floor is visited you track if evelator is going up or down and resort events

ElevatorEvents
{
  Timestamp (Date and time including seconds) - use for timing
  DirectionOfElevator is Up or Down
  DirectionOfEventUp bool used to determine if elevator will hit this floor when going up if true
  FloorRequestedBool bool used when floor is Requested
  FloorOn int is floor elevator has moved to
  FloorPassedBool bool used when floor is passed
  FloorStopped bool used when the floor is stoppedon
  CompletedEvent bool event is done
}

-other items to keep in mind can add a floor async, add way to gather input

example
go to floor 2, 8, 5 note start on 1.

idea is to go through elevator events, elevator events are key driver for how elevator will move
-go up or go down

idea the elevator will contain elevator events with floors above current floor, floors below current floor, completedEvents aka moved to completed events when the floor is reached

so you determine if event is above or below current floor if same floor nothing happens, just log same floor requested and have a record

// now the elevator, be able to gather inputs of what floors to visit.
-once floors are gathered, turn that into event objects for tdd

-do the timer last for now just add floors to visit and sort the floors in order, keep going the needed direction


//notes on what I have
I can collect floors to visit, this is made into event objects and events sorted ascending by floor are availible.


// new idea focus on the direction the elevator goes and what happens between the floors
Up, Down, Stopped(Idle)