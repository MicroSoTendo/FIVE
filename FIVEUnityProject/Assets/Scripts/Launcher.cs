using System;
using System.Collections.Generic;
using UnityEngine;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;

namespace FIVE
{
    public class Launcher : MonoBehaviour
    {
        public List<GameObject> InfranstructuresOnAwake;
        public List<GameObject> InfranstructuresOnStart;
        private void Awake()
        {
            EventManager.RaiseEvent<OnLauncherAwake>(this, EventArgs.Empty);
            InfranstructuresOnAwake.ForEach(o => { Instantiate(o); });
        }
        private void Start()
        {
            EventSystem.EventManager.RaiseEvent<OnLauncherStart>(this, EventArgs.Empty);
            InfranstructuresOnStart.ForEach(o => { Instantiate(o); });
        }
        private void Update()
        {
            EventSystem.EventManager.RaiseEvent<OnLauncherUpdate>(this, EventArgs.Empty);
        }
    }
}