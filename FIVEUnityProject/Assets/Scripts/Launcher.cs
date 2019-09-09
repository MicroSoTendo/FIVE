using System;
using System.Collections.Generic;
using UnityEngine;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;
using System.Collections;

namespace FIVE
{
    public class Launcher : MonoBehaviour
    {
        public List<GameObject> InfranstructuresOnAwake;
        public List<GameObject> InfranstructuresOnStart;

        private void Awake()
        {
            EventManager.RaiseEvent<OnLauncherAwake>(this, EventArgs.Empty);
            InfranstructuresOnAwake.ForEach(v => Instantiate(v));
        }

        private IEnumerator Start()
        {
            EventSystem.EventManager.RaiseEvent<OnLauncherStart>(this, EventArgs.Empty);
            foreach(var prefab in InfranstructuresOnStart)
            {
                Instantiate(prefab);
                yield return null;
            }
        }

        private void Update()
        {
            EventSystem.EventManager.RaiseEvent<OnLauncherUpdate>(this, EventArgs.Empty);
        }
    }
}