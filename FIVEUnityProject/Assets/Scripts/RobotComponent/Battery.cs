using UnityEngine;

namespace FIVE.RobotComponent
{
    public class Battery : MonoBehaviour
    {
        public float Capacity;
        public float CurrentEnergy;
        private bool isCharging;
        public int DechargeSpeed;
        private int chargeSpeed;

        public void Start()
        {
            Capacity = 100.0f;
            CurrentEnergy = Capacity;
            DechargeSpeed = 1;
        }

        public void Update()
        {
            if (isCharging)
            {
                CurrentEnergy += Mathf.Clamp(chargeSpeed * Time.deltaTime, 0, Capacity);
            }
            else
            {
                CurrentEnergy -= Mathf.Clamp(DechargeSpeed * Time.deltaTime, 0, Capacity);
            }
        }

        public void Charge(int chargeSpeed)
        {
            isCharging = true;
            this.chargeSpeed = chargeSpeed;
        }

        public void UnCharge()
        {
            isCharging = false;
        }
    }


}