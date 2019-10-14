using System;
using FIVE.EventSystem;
using UnityEngine;

namespace FIVE.RobotComponents
{
    public class Battery : RobotComponent
    {
        private float capacity;
        private int chargeSpeed;

        private float currentEnergy;
        public int DechargeSpeed;

        private bool isCharging;

        public float Capacity
        {
            get => capacity;
            set
            {
                capacity = value;
            }
        }

        public float CurrentEnergy
        {
            get => currentEnergy;
            set
            {
                currentEnergy = value;
                this.RaiseEvent<OnRobotEnergyChanged, RobotEnergyChangedEventArgs>(
                    new RobotEnergyChangedEventArgs(value));
            }
        }

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

    public class RobotEnergyChangedEventArgs : EventArgs
    {
        public float NewEnergyLevel { get; }
        public RobotEnergyChangedEventArgs(float newEnergyLevel) => NewEnergyLevel = newEnergyLevel;
    }

    public abstract class OnRobotEnergyChanged : IEventType<RobotEnergyChangedEventArgs>
    {
    }
}