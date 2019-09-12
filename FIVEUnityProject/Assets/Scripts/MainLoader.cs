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
        public List<GameObject> InfrastructuresOnAwake;
        public List<GameObject> InfrastructuresOnStart;

        private void Awake()
        {
            this.RaiseEvent<OnLauncherAwake>(EventArgs.Empty);
            InfrastructuresOnAwake.ForEach(v => Instantiate(v));
        }

        private IEnumerator Start()
        {
            this.RaiseEvent<OnLauncherStart>(EventArgs.Empty);
            foreach (var prefab in InfrastructuresOnStart)
            {
                Instantiate(prefab);
                yield return null;
            }
            Destroy(this);
            this.RaiseEvent<OnLauncherDestroyed>(EventArgs.Empty);
        }
    }
}