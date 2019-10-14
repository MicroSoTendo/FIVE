using System;
using Mirror;
using UnityEngine;

namespace FIVE.Robot
{
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(NetworkTransformChild))]
    public class RobotBehaviour : NetworkBehaviour
    {
        private NetworkTransform networkTransform;
        private NetworkTransformChild networkTransformChild;
        private NetworkAnimator networkAnimator;

        protected Action OnLocalPlayerUpdate;

        protected virtual void Awake()
        {
            OnLocalPlayerUpdate += () => { }; //Avoid null checking
            networkTransform = GetComponent<NetworkTransform>();
            networkTransformChild = GetComponent<NetworkTransformChild>();
            networkAnimator = GetComponent<NetworkAnimator>();
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (!isLocalPlayer)
            {
                OnLocalPlayerUpdate();
            }
        }
    }
}
