using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace FIVE.EventSystem
{
    internal sealed class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> ScheduledActions = new ConcurrentQueue<Action>();
        private Action onUpdate = () => { };
        public Action OnUpdate
        {
            get => onUpdate;
            set { onUpdate = value ?? (() => { }); }
        }

        void Update()
        {
            onUpdate.Invoke();
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
