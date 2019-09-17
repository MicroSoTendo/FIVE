using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

namespace FIVE.EventSystem
{
    internal sealed class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> ScheduledActions = new ConcurrentQueue<Action>();
        private static readonly ConcurrentQueue<IEnumerator> ScheduledCoroutine = new ConcurrentQueue<IEnumerator>();
        private Action onUpdate = () => { };

        public Action OnUpdate
        {
            get => onUpdate;
            set => onUpdate = value ?? (() => { });
        }

        private void Update()
        {
            onUpdate.Invoke();
            while (!ScheduledActions.IsEmpty)
            {
                ScheduledActions.TryDequeue(out Action result);
                result();
            }
            while (!ScheduledCoroutine.IsEmpty)
            {
                ScheduledCoroutine.TryDequeue(out IEnumerator result);
                StartCoroutine(result);
            }
        }

        public static void Schedule(Action action)
        {
            ScheduledActions.Enqueue(action);
        }

        public static void ScheduleCoroutine(IEnumerator coroutine)
        {
            ScheduledCoroutine.Enqueue(coroutine);
        }
    }
}