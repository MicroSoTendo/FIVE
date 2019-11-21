using FIVE.EventSystem;
using FIVE.Robot;
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
                currentEnergy = Mathf.Clamp(value, 0f, Capacity);
                this.RaiseEvent<OnRobotEnergyChanged, RobotEnergyChangedEventArgs>(new RobotEnergyChangedEventArgs(CurrentEnergy));
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
                CurrentEnergy += chargeSpeed;
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