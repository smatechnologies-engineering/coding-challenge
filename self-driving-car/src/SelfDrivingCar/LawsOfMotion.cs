namespace SelfDrivingCar
{
    public class LawsOfMotion
    {
        public static double GetFinalVelocity(double initialVelocity, double acceleration, double elapsedTime)
        {
            // v = u + a*t
            return initialVelocity + acceleration * elapsedTime;
        }

        public static double GetTimeToZeroVelocity(double initialVelocity, double acceleration)
        {
            // t = (v - u)/a
            return (0 - initialVelocity) / acceleration;
        }

        public static double GetTimeToMaxVelocity(double initialVelocity, double acceleration, int maxVelocity)
        {
            // t = (v - u)/a
            return (maxVelocity - initialVelocity) / acceleration;
        }

        public static double GetDistanceTravelled(double initalVelocity, double finalVelocity, double elapsedTime)
        {
            // s = t*(u + v)/2
            return elapsedTime * (initalVelocity + finalVelocity) / 2;
        }
    }
}
