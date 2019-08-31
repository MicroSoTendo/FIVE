using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.EventSystem
{
    class Wrapper : MonoBehaviour
    {
        enum ExecutingMethod
        {
            Coroutine,
            Update,
            Asynchronous
        }

        [SerializeField] private ExecutingMethod executingMethod = ExecutingMethod.Coroutine;
        void Start()
        {
            if(executingMethod == ExecutingMethod.Coroutine)
                StartCoroutine(EventSystem.EventSystemCoroutine());
            if (executingMethod == ExecutingMethod.Asynchronous)
                EventSystem.RunAsync();
        }

        void Update()
        {
            if(executingMethod == ExecutingMethod.Update)
                EventSystem.RunOnce();
        }
    }
}
