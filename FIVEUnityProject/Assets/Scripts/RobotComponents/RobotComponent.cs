using UnityEngine;

namespace FIVE.RobotComponents
{
    public class RobotComponent : MonoBehaviour
    {
        public float PowerConsumption { get; protected set; }

        private void Update()
        {
            GetComponent<Battery>().CurrentEnergy -= PowerConsumption * Time.deltaTime;
        }
    }
}