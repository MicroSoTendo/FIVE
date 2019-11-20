using FIVE.EventSystem;
using System;
using UnityEngine;

namespace FIVE.RobotComponents
{
    public class Battery : RobotComponent
    {
        private int chargeSpeed;

        private float currentEnergy;

        private bool isCharging;

        public float Capacity { get; set; }

        public float CurrentEnergy
        {
            get => currentEnergy;
            set
            {
                currentEnergy = value;
                this.RaiseEvent<OnRobotEnergyChanged, RobotEnergyChangedEventArgs>(new RobotEnergyChangedEventArgs(value));
            }
        }

        public void Start()
        {
            Capacity = 100.0f;
            CurrentEnergy = Capacity;
            PowerConsumption = 0.1f;
        }

        public void Update()
        {
            if (isCharging)
            {
                currentEnergy += chargeSpeed;
            }
            foreach (RobotComponent c in GetComponents<RobotComponent>())
            {
                currentEnergy -= c.PowerConsumption * Time.deltaTime;
            }
            CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, Capacity);
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

    public class RobotEnergyChangedEventArgs : EventArgs
    {
        public float NewEnergyLevel { get; }

        public RobotEnergyChangedEventArgs(float newEnergyLevel)
        {
            NewEnergyLevel = newEnergyLevel;
        }
    }

    public abstract class OnRobotEnergyChanged : IEventType<RobotEnergyChangedEventArgs>
    {
    }
}