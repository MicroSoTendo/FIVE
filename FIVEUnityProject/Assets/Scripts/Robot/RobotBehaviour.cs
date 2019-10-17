using System;
using FIVE.GameModes;
using UnityEngine;

namespace FIVE.Robot
{
    public class RobotBehaviour : MonoBehaviour
    {
        //private NetworkAnimator networkAnimator;
        //private NetworkTransform networkTransform;
        //private NetworkTransformChild networkTransformChild;

        /// <summary>
        /// Update callbacks only happens on local player.
        /// </summary>
        protected Action OnLocalPlayerUpdate;
        protected Action OnUpdateGlobal;

        protected virtual void Awake()
        {
            OnUpdateGlobal += () => { };
            OnLocalPlayerUpdate += () => { }; //Avoid null checking
            //networkTransform = GetComponent<NetworkTransform>();
            //networkTransformChild = GetComponent<NetworkTransformChild>();
            //networkAnimator = GetComponent<NetworkAnimator>();
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            //if (Entry.CurrentMode == Entry.Mode.Multi && isLocalPlayer)
            //{
            //    return;
            //}
            OnLocalPlayerUpdate();
        }
    }
}