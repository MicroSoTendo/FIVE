//using Mirror;
using UnityEngine;

namespace FIVE.RobotComponents
{
    public class SolarSingleton
    {
        public static float PowerCharge = 0.0f;
    }

    public class Solar : RobotComponent
    {
        private void Update()
        {
            PowerConsumption = -SolarSingleton.PowerCharge;
        }
    }
}