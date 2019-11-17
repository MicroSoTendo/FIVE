using System;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotBehaviour : MonoBehaviour
    {
        protected Action OnFixedUpdate;
        protected virtual void Awake()
        {
            OnFixedUpdate += () => { }; //Avoid null checking
        }

        protected virtual void Start()
        {
        }
        protected virtual void FixedUpdate()
        {
            OnFixedUpdate();
        }
    }
}