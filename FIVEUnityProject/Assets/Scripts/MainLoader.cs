using FIVE.EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    public class MainLoader : MonoBehaviour
    {
        public List<GameObject> InfrastructuresOnAwake;
        public List<GameObject> InfrastructuresOnStart;

        private void Awake()
        {
            this.RaiseEvent<OnMainLoaderAwake>(EventArgs.Empty);
            InfrastructuresOnAwake.ForEach(v =>
            {
                DontDestroyOnLoad(Instantiate(v));
            });
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