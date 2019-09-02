using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Assets.Scripts.EventSystem
{
    internal sealed class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> ScheduledActions =
            new ConcurrentQueue<Action>();

        void Update()
        {
            while (!ScheduledActions.IsEmpty)
            {
                ScheduledActions.TryDequeue(out var result);
                result();
            }
        }

        public static void Schedule(Action action)
        {
            ScheduledActions.Enqueue(action);
        }
    }
}
