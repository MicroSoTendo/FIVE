using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Assets.Scripts.EventSystem
{
    internal sealed class MainThreadEventDispatcher : MonoBehaviour
    {
        private static readonly ConcurrentQueue<(EventHandler action, object sender, EventArgs eventArgs)> ScheduledEvent =
            new ConcurrentQueue<(EventHandler action, object sender, EventArgs eventArgs)>();

        void Update()
        {
            while (!ScheduledEvent.IsEmpty)
            {
                ScheduledEvent.TryDequeue(out var result);
                result.action?.Invoke(result.action, result.eventArgs);
            }
        }

        public static void ScheduleEvent(EventHandler action, object sender, EventArgs eventArgs)
        {
            ScheduledEvent.Enqueue((action,sender,eventArgs));
        }
    }
}
