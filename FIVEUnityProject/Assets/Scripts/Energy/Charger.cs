using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FIVE.Robot;

namespace FIVE
{
    public class Charger : MonoBehaviour
    {
        public int ChargeSpeed { get; set; }

        private void Start()
        {
            ChargeSpeed = 10;
        }

        private void OnTriggerEnter(Collider other)
        {
            RobotSphere robotSphere = other.gameObject.GetComponent<Robot.RobotSphere>();
            if (robotSphere != null)
            {
                //robotSphere.Battery.Charge(ChargeSpeed);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            RobotSphere robotSphere = other.gameObject.GetComponent<Robot.RobotSphere>();
            if (robotSphere != null)
            {
                //robotSphere.Battery.UnCharge();
            }
        }
    }
}
