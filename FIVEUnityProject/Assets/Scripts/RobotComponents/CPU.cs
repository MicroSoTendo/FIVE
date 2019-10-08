using Mirror;
using UnityEngine;

namespace FIVE.RobotComponents
{
    public class CPU : RobotComponent
    {

        public int Speed; // instruction executed per frame

        [SyncVar]
        [Range(0f, 1f)]
        public float PowerConsumption;

        private void Start()
        {
            Speed = 1;
            PowerConsumption = 1.0f;
        }
    }
}