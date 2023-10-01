using CoreElevator;
using Konsole;
using System;
using System.Threading;

// create an 80 by 20 inline window
var window = new Window().Concurrent();
var w = Window.OpenBox("Elevator 3000");

//var top = window.SplitTop("Elevator 3000");
//var bottom = window.SplitBottom("Commands");
// split that window into boxes
var queueWin = window.SplitLeft("Pending Floor Requests");
var status = window.SplitRight("Elevator Status");
var tsConsole = new ConcurrentWriter(status);
var commands = window.OpenBox("Commands", 0, window.WindowHeight-1, window.WindowWidth, 10);

// right and left are threadsafe, window is not.

//create elevator
ElevatorController elevator = new CoreElevator.ElevatorController(tsConsole, queueWin);

StartAsync(elevator, commands, tsConsole, queueWin);
//set elevator state floor

//accept new floor request async
//FloorRequest floorRequest = new FloorRequest("6U");
//FloorRequests floorRequests = new FloorRequests();
//elevator.addRequest("6U");

//floorRequests.Add(floorRequest, elevator.currentState);


//start moving elevator to requested floor
//stop at requested floor and wait specified time
//continue listening for additional floor requests(internal or external)

static async void StartAsync(ElevatorController elevator, IConsole bottom, ConcurrentWriter status, IConsole queueWin)
{
    status.WriteLine("Floor: " + elevator.currentState.CurrentFloor + " : " + elevator.currentState.Status);

    string? userInput;
    var cmdWindow = Window.Open(bottom);
    cmdWindow.Write("Enter Floor/Direction (i.e. 12, 12U):");
    userInput = Console.ReadLine();

    while (true)
    {
        if (userInput?.ToUpper() == "Q")
        {
            //make sure all previous requests are fullfilled first. 
            while (elevator.requests.TotalCurrentRequests() > 0)
            { 
                //sleep for one second on each check so we don't check too often.. for performance.
                Thread.Sleep(1000); 
            }
            Environment.Exit(0);
        }
        else
        {
            if (!String.IsNullOrEmpty(userInput))
            {
                elevator.addRequest(userInput);
            }
            cmdWindow = Window.Open(bottom);
            cmdWindow.Write("Enter Floor/Direction (i.e. 12, 12U):");
            userInput = Console.ReadLine();
        }

    }
}