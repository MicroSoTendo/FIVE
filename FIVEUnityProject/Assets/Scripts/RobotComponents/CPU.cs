//using Mirror;
using UnityEngine;

namespace FIVE.RobotComponents
{
    public class CPU : RobotComponent
    {
        [Range(0f, 1f)] public float PowerConsumption;

        public int Speed; // instruction executed per frame

        private void Start()
        {
            Speed = 1;
            PowerConsumption = 1.0f;
        }
    }
}