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
            this.RaiseEvent<OnMainLoaderAwake>(EventArgs.Empty);
            InfrastructuresOnAwake.ForEach(v => Instantiate(v));
        }

        private IEnumerator Start()
        {
            this.RaiseEvent<OnMainLoaderStart>(EventArgs.Empty);
            foreach (GameObject prefab in InfrastructuresOnStart)
            {
                Instantiate(prefab);
                yield return null;
            }
            Destroy(this);
            this.RaiseEvent<OnMainLoaderDestroyed>(EventArgs.Empty);
        }
    }
}