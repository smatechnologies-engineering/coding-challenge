using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Konsole;

namespace CoreElevator
{
    public class ElevatorController
    {
        public FloorRequests requests;
        public ElevatorState currentState;
        public int totalFloors = 50;
        private IConsole queueWin;
        private ConcurrentWriter status;
        public LogEntry logEntry;

        public ElevatorController(ConcurrentWriter? Status, IConsole? QueueWin)
        {
            currentState = new ElevatorState();
            if(QueueWin != null)
            {
                queueWin = QueueWin;
            }
            if(Status != null)
            {
                status = Status;
            }
            
            requests = new FloorRequests(queueWin);
            logEntry = new LogEntry();
        }

        async public Task<bool> addRequest(string request)
        {
            FloorRequest newRequest = new FloorRequest(request, totalFloors);

            //if we're already on the floor requested, no need to add it to the list
            if (newRequest.floor != currentState.CurrentFloor && newRequest.floor > 0)
            {

                requests.Add(newRequest, currentState, totalFloors);

                if(requests.downRequests.Count + requests.upRequests.Count + requests.insideRequests.Count <= 1)
                {
                    while(requests.downRequests.Count + requests.upRequests.Count + requests.insideRequests.Count > 0)
                    {
                        await move();
                    }
                }
                
            }
            
            return true;
        }

        async private Task<bool> move()
        {
            if (currentState.Status == StateType.Free)
            {
                if (requests.upRequests.Count > 0)
                {
                    int requestedFloor = requests.upRequests[0];
                    // make sure floor isn't zero or lower, should have already been checked, but double checking
                    if (requestedFloor > 0)
                    {
                        //change to floor
                        if (setFloor(requests.upRequests[0], StateType.Up) || hasInsideFloorRequest(true))
                        {
                            await processFloor(EventAction.Wait);
                        }
                        else
                        {
                            await processFloor(EventAction.Pass);
                        }

                    }
                }
                else if (requests.downRequests.Count > 0)
                {
                    int requestedFloor = requests.downRequests[0];
                    // make sure floor isn't zero or lower, should have already been checked, but double checking
                    if (requestedFloor > 0)
                    {
                        //change to floor
                        if (setFloor(requests.downRequests[0], StateType.Down) || hasInsideFloorRequest(true))
                        {
                            await processFloor(EventAction.Wait);
                        }
                        else
                        {
                            await processFloor(EventAction.Pass);
                        }

                    }
                }
                else if(requests.insideRequests.Count > 0)
                {
                    int requestedFloor = requests.insideRequests[0];

                    if (requestedFloor > currentState.CurrentFloor)
                    {
                        //change to floor
                        if (setFloor(requestedFloor, StateType.Up, true))
                        {
                            await processFloor(EventAction.Wait);
                        }
                        else
                        {
                            await processFloor(EventAction.Pass);
                        }

                    }
                    else if (requestedFloor < currentState.CurrentFloor)
                    {
                        //change to floor
                        if (setFloor(requestedFloor, StateType.Down, true))
                        {
                            await processFloor(EventAction.Wait);
                        }
                        else
                        {
                            await processFloor(EventAction.Pass);
                        }
                    }
                   
                }
                else
                {

                }
            }
            else if (currentState.Status == StateType.Up)
            {
               if (requests.upRequests.Count > 0)
                {
                    int requestedFloor = requests.upRequests[0];
                    // make sure floor isn't zero or lower, should have already been checked, but double checking
                    if (requestedFloor > 0)
                    {
                        //change to floor
                        if (setFloor(requests.upRequests[0], StateType.Up) || hasInsideFloorRequest(true))
                        {
                            await processFloor(EventAction.Wait);
                        }
                        else
                        {
                            await processFloor(EventAction.Pass);
                        }

                    }
                }
                //check inside requests for higher floor as they take priority
                else if(hasInsideRequestThisDirection(StateType.Up))
                {
                    int requestedFloorIndex = getInsideRequestThisDirection(StateType.Up);
                    int requestedFloor = requests.insideRequests[requestedFloorIndex];
                    //change to floor
                    if (setFloor(requestedFloor, StateType.Up, true))
                    {
                        await processFloor(EventAction.Wait);
                    }
                    else
                    {
                        await processFloor(EventAction.Pass);
                    }
                }
                else if (requests.downRequests.Count > 0)
                {
                    // we emptied the previous direction, time to check the other
                    currentState.Status = StateType.Down;
                }
                //check inside requests for lower floor as they take priority
                else if (hasInsideRequestThisDirection(StateType.Down))
                {
                    int requestedFloorIndex = getInsideRequestThisDirection(StateType.Down);
                    int requestedFloor = requests.insideRequests[requestedFloorIndex];
                    //change to floor
                    if (setFloor(requestedFloor, StateType.Down, true))
                    {
                        await processFloor(EventAction.Wait);
                    }
                    else
                    {
                        await processFloor(EventAction.Pass);
                    }
                }
                else
                {
                    currentState.Status = StateType.Free;
                }
            }
            else if (currentState.Status == StateType.Down)
            {
                if (requests.downRequests.Count > 0)
                {
                    int requestedFloor = requests.downRequests[0];
                    // make sure floor isn't zero or lower, should have already been checked, but double checking
                    if (requestedFloor > 0)
                    {
                        //change to floor
                        if (setFloor(requests.downRequests[0], StateType.Down) || hasInsideFloorRequest(true))
                        {
                            await processFloor(EventAction.Wait);
                        }
                        else
                        {
                            await processFloor(EventAction.Pass);
                        }

                    }
                }
                //check inside requests for lower floor as they take priority
                else if (hasInsideRequestThisDirection(StateType.Down))
                {
                    int requestedFloorIndex = getInsideRequestThisDirection(StateType.Down);
                    int requestedFloor = requests.insideRequests[requestedFloorIndex];
                    //change to floor
                    if (setFloor(requestedFloor, StateType.Down, true))
                    {
                        await processFloor(EventAction.Wait);
                    }
                    else
                    {
                        await processFloor(EventAction.Pass);
                    }
                }
                else if (requests.upRequests.Count > 0)
                {
                    // we emptied the previous direction, time to check the other
                    currentState.Status = StateType.Up;
                }
                //check inside requests for higher floor as they take priority
                else if (hasInsideRequestThisDirection(StateType.Up))
                {
                    int requestedFloorIndex = getInsideRequestThisDirection(StateType.Up);
                    int requestedFloor = requests.insideRequests[requestedFloorIndex];
                    //change to floor
                    if (setFloor(requestedFloor, StateType.Up, true))
                    {
                        await processFloor(EventAction.Wait);
                    }
                    else
                    {
                        await processFloor(EventAction.Pass);
                    }
                }
                else
                {
                    currentState.Status = StateType.Free;
                }
                
            }
            if(queueWin != null)
                requests.outputLists();
            return true;
        }
        enum EventAction
        {
            Pass,
            Wait
        }
        private async Task processFloor(EventAction eventAction)
        {
            if(eventAction == EventAction.Wait)
            {
                if(status != null)
                    status.WriteLine(ConsoleColor.Green, "Waiting at Floor " + currentState.CurrentFloor);
                //Log the event
                logEntry.WriteLog(currentState.CurrentFloor.ToString() + " - Waiting", LogEntry.logType.FloorEvent);
                //wait one second
                await Task.Delay(1000);
            }
            else if(eventAction == EventAction.Pass)
            {
                if (status != null)
                    status.WriteLine(ConsoleColor.Yellow, "Currently Passed Floor " + currentState.CurrentFloor);
                //Log the event
                logEntry.WriteLog(currentState.CurrentFloor.ToString() + " - Passing", LogEntry.logType.FloorEvent);
                //wait 3 seconds
                await Task.Delay(3000);
            }
        }
        private bool hasInsideRequestThisDirection(StateType direction)
        {
            return getInsideRequestThisDirection(direction) >= 0 ? true : false;
        }
        private int getInsideRequestThisDirection(StateType direction)
        {
            if (direction == StateType.Up)
            {
                for (int i = 0; i <= requests.insideRequests.Count - 1; i++)
                {
                    if (requests.insideRequests[i] > currentState.CurrentFloor)
                    {
                        return i;
                    }

                }
                return -1;
            }
            else if (direction == StateType.Down)
            {
                for (int i = requests.insideRequests.Count - 1; i >= 0; i--)
                {
                    if (requests.insideRequests[i] < currentState.CurrentFloor)
                    {
                        return i;
                    }

                }
                return -1;
            }
            else
            {
                return -1;
            }

        }
        private bool hasInsideFloorRequest(bool alsoRemove)
        {
            if(requests.insideRequests.Contains(currentState.CurrentFloor))
            {
                // since the current floor exists as an open request from a current rider we to stop
                if(alsoRemove)
                {
                    requests.insideRequests.RemoveAt(requests.insideRequests.IndexOf(currentState.CurrentFloor));
                }
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool setFloor(int nextFloor, StateType state, bool isInsideRequest = false)
        {
            currentState.Status = state;
            
            if(state == StateType.Up)
                currentState.CurrentFloor =  nextFloor >= currentState.CurrentFloor?
                    currentState.CurrentFloor + 1: currentState.CurrentFloor - 1;
            else if(state == StateType.Down)
                currentState.CurrentFloor = nextFloor <= currentState.CurrentFloor ?
                    currentState.CurrentFloor - 1 : currentState.CurrentFloor + 1;

            if (currentState.CurrentFloor == nextFloor)
            {
                //we reached the next targeted floor, remove it from the list
                if(state == StateType.Up)
                {
                    if(!isInsideRequest)
                    {
                        requests.upRequests.RemoveAt(0);
                    }
                    else
                    {
                        requests.insideRequests.RemoveAt(requests.insideRequests.IndexOf(currentState.CurrentFloor));
                    }
                    
                    if (requests.upRequests.Count == 0 && requests.downRequests.Count == 0 && requests.insideRequests.Count == 0)
                    {
                        currentState.Status = StateType.Free;

                    }
                }
                else if(state == StateType.Down)
                {
                    if (!isInsideRequest)
                    {
                        requests.downRequests.RemoveAt(0);
                    }
                    else
                    {
                        requests.insideRequests.RemoveAt(requests.insideRequests.IndexOf(currentState.CurrentFloor));
                    }

                    if (requests.upRequests.Count == 0 && requests.downRequests.Count == 0 && requests.insideRequests.Count == 0)
                    {
                        currentState.Status = StateType.Free;

                    }
                }
                return true;
            }
            else
            {
                //didn't reach the floor yet
                return false;
            }
        }
        
    }

    public class ElevatorState
    {
        private StateType status = StateType.Free;
        private int currentFloor = 1;

        public StateType Status
        {
            get { return status; }
            set { status = value; }
        }
        public int CurrentFloor
        {
            get { return currentFloor; }
            set { currentFloor = value; }
        }

    }
    public class FloorRequests
    {
        public List<int> upRequests = new List<int>();
        public List<int> downRequests = new List<int>();
        public List<int> insideRequests = new List<int>();
        private LogEntry logEntry = new LogEntry();

        private IConsole queueWin;

        public FloorRequests(IConsole? QueueWin)
        {
            if(QueueWin != null)
                queueWin = QueueWin;
        }
        public int TotalCurrentRequests()
        {
            return upRequests.Count + downRequests.Count + insideRequests.Count;
        }
        public void Add(FloorRequest request, ElevatorState elevatorState, int totalFloors)
        {
            //First check for a request that has direction regardless of floor
            if(request.direction == Direction.None)
            {
                //since there isn't a direction, it's a choice made from INSIDE the elevator
                insideRequests.Add(request.floor);
            }
            else if(request.direction == Direction.Down)
            {
                downRequests.Add(request.floor);
            }
            else if(request.direction == Direction.Up)
            {
                upRequests.Add(request.floor);
            }
            // all floor requests below should be of type None which means it was selected from INSIDE the elevator
            else if (elevatorState.Status == StateType.Free || elevatorState.Status == StateType.None)
            {
                if (request.floor > elevatorState.CurrentFloor)
                {
                    upRequests.Add(request.floor);
                }
                else if (request.floor < elevatorState.CurrentFloor)
                {
                    downRequests.Add(request.floor);
                }
            }
            else if(elevatorState.Status == StateType.Up)
            {
                if (request.floor > elevatorState.CurrentFloor)
                {
                    upRequests.Add(request.floor);
                }
                else if (request.floor < elevatorState.CurrentFloor)
                {
                    downRequests.Add(request.floor);
                }
            }
            else if (elevatorState.Status == StateType.Down)
            {
                if (request.floor < elevatorState.CurrentFloor)
                {
                    downRequests.Add(request.floor);
                }
                else if (request.floor > elevatorState.CurrentFloor)
                {
                    upRequests.Add(request.floor);
                }
            }
            //Log the request
            string logFloorDirection = request.direction != Direction.None ? request.direction.ToString() : "";
            logEntry.WriteLog(request.floor.ToString() + logFloorDirection, LogEntry.logType.FloorRequest);

            //sort Lists accordingly
            sortList(elevatorState);
            if(queueWin != null)
                outputLists();

        }
        public void outputLists()
        {
            if(queueWin != null)
            {
                queueWin.Clear();
                queueWin.WriteLine(ConsoleColor.Blue, "Inside Rider Requests:");
                insideRequests.ForEach(r => queueWin.WriteLine("\t" + r));
                queueWin.WriteLine(ConsoleColor.Blue, "Pending Up Requests:");
                upRequests.ForEach(r => queueWin.WriteLine("\t" + r));
                queueWin.WriteLine(ConsoleColor.Blue, "Pending Down Requests:");
                downRequests.ForEach(r => queueWin.WriteLine("\t" + r));
            }
            

        }
        public void sortList(ElevatorState elevatorState)
        {
            upRequests.Sort();
            int upRequestsLength = upRequests.Count;
            //List<int> newUpList = new List<int>();
            for (int i=0; i <= upRequestsLength - 1; i++)
            {
                if (upRequests[0] <= elevatorState.CurrentFloor)
                {
                    int item = upRequests[0];
                    upRequests.RemoveAt(0);
                    upRequests.Add(item);
                    
                }
                else
                {
                    // we reached a number that is in the right order, let's bail out
                    break;
                }
            }

            downRequests.Sort();
            downRequests.Reverse();
            int downRequestsLength = downRequests.Count;
            //int counter = 0;
            for (int i=0; i <= downRequestsLength - 1; i++)
            {
                if (downRequests[0] >= elevatorState.CurrentFloor)
                {
                    int item = downRequests[0];
                    downRequests.RemoveAt(0);
                    downRequests.Add(item);
                    
                }
                else
                {
                    // we reached a number that is in the right order, let's bail out
                    break;
                }
            }
            // sort normal first
            insideRequests.Sort();
            int insideRequestsLength = insideRequests.Count;
            
            //remove dups
            upRequests = upRequests.Distinct().ToList();
            downRequests = downRequests.Distinct().ToList();
            insideRequests = insideRequests.Distinct().ToList();

        }
    }
    public class FloorRequest
    {
        public int floor
        {
            get; set;
        }
        public Direction direction
        {
            get; set;
        }

        public FloorRequest(string request, int totalFloors)
        {
            // parse the request to floor and possible direction
            if(request.Length > 0)
            {
                //check if we have a letter as the last char for our direction
                // if not, then the button press is from inside the elevator
                char lastLetter = request[request.Length - 1];
                if (Char.IsLetter(lastLetter))
                {
                    // make sure we have a floor number as a minimum requirement
                    direction = checkDirection(lastLetter);

                    floor = checkFloor(request.Substring(0, request.Length - 1), totalFloors);
                }
                else
                {
                    // make sure we have a floor number as a minimum requirement
                    floor = checkFloor(request, totalFloors);
                }
                
            }
        }
        private bool isValidFloor(int floorNum, int totalFloors)
        {
            return floorNum > 0 && floorNum <= totalFloors;
        }
        private int checkFloor( string request, int totalFloors)
        {
            // make sure we have a floor number as a minimum requirement
            int floorNum = -1;
            bool isNum = int.TryParse(request, out floorNum);
            if ( isNum && isValidFloor(floorNum, totalFloors))
            {
                return floorNum;
            }
            //raise error at some point
            return 0;
        }
        private Direction checkDirection(char lastLetter)
        {
            switch (lastLetter.ToString().ToUpper())
            {
                case "U":
                    return Direction.Up;
                    
                case "D":
                    return Direction.Down;
                    
            }
            return Direction.None;
        }
    }
    public enum StateType
    {
        None = 0,
        Up = 1,
        Down = 2,
        Free = 3,
        //BusyWaiting = 4
    }
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2
    }
}