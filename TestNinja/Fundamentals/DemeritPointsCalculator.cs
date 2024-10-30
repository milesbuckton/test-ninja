using System;

namespace TestNinja.Fundamentals
{
    public class DemeritPointsCalculator
    {
        private const int SpeedLimit = 65;
        private const int MaxSpeed = 300;

        public int CalculateDemeritPoints(int speed)
        {
            switch (speed)
            {
                case < 0:
                case > MaxSpeed:
                    throw new ArgumentOutOfRangeException();
                case <= SpeedLimit:
                    return 0;
            }

            const int kmPerDemeritPoint = 5;
            var demeritPoints = (speed - SpeedLimit) / kmPerDemeritPoint;

            return demeritPoints;
        }
    }
}
