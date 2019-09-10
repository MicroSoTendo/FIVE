using System;
using System.Collections.Generic;
using UnityEngine;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;
using System.Collections;

namespace FIVE
{
    public class MainLoader : MonoBehaviour
    {
        public List<GameObject> InfranstructuresOnAwake;
        public List<GameObject> InfranstructuresOnStart;

        private void Awake()
        {
            this.RaiseEvent<OnLauncherAwake>(EventArgs.Empty);
            InfranstructuresOnAwake.ForEach(v => Instantiate(v));
        }

        private IEnumerator Start()
        {
            this.RaiseEvent<OnLauncherStart>(EventArgs.Empty);
            foreach (var prefab in InfranstructuresOnStart)
            {
                Instantiate(prefab);
                yield return null;
            }
            Destroy(this);
            this.RaiseEvent<OnLauncherDestroyed>(EventArgs.Empty);
        }
    }
}