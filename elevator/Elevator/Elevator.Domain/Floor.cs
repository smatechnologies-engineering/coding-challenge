using System;

namespace Elevator.Domain
{
    public class Floor
    {
        private ILogging logging;
        public int Level { get; set; }
        public bool DownButtonPressed { get; private set; }
        public bool UpButtonPressed { get; private set; }
        public bool InternalButtonPress { get; private set; }

        public Floor(int location)
        {
            this.Level = location;
            logging = new Logging();
        }

        private void GoDown()
        {
            logging.LogWithTime($"Level {Level} needs to go down");
            this.DownButtonPressed = true;
        }

        private void GoUp()
        {
            logging.LogWithTime($"Level {Level} needs to go up");
            this.UpButtonPressed = true;
        }

        public void ClearFloor()
        {
            DownButtonPressed = false;
            UpButtonPressed = false;
            InternalButtonPress = false;
        }

        public void ButtonPressedFromElevator()
        {
            logging.LogWithTime($"Button Pressed from inside of the elevator for Level: {this.Level}");
            InternalButtonPress = true;
        }

        public void ButtonPressed(string direction)
        {
            switch (direction.ToLowerInvariant())
            {
                case "u":
                    GoUp();
                    break;
                case "d":
                    GoDown();
                    break;
                default:
                    Console.WriteLine("Congrats, you broke it!");
                    return;

            }
        }
    }
}
