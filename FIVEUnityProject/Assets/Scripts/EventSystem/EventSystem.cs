using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Script.EventSystem
{
    internal sealed class EventSystem
    {
        private readonly EventHandler[] listOfHandlers = new EventHandler[Enum.GetNames(typeof(EventTypes)).Length];
        private readonly Queue<(EventTypes eventTypes, object sender, EventArgs eventArgs)> requestsQueue = new Queue<(EventTypes eventTypes, object sender, EventArgs eventArgs)>();
        private static EventSystem Instance { get; } = new EventSystem();

        public static void Subscribe(EventTypes eventTypes, EventHandler del)
        {
            Instance.listOfHandlers[(uint)eventTypes] += del;
        }

        public static EventHandler GetHandlers(EventTypes eventTypes)
        {
            return Instance.listOfHandlers[(int)eventTypes];
        }

        public static void RaiseEvent(EventTypes eventTypes, object sender, EventArgs eventArgs)
        {
            Instance.requestsQueue.Enqueue((eventTypes, sender, eventArgs));
        }

        public static IEnumerator EventSystemCoroutine()
        {
            while (true)
            {
                while (Instance.requestsQueue.Count != 0)
                {
                    var (eventTypes, sender, eventArgs) = Instance.requestsQueue.Dequeue();
                    var del = Instance.listOfHandlers[(uint)eventTypes];
                    del?.Invoke(sender, eventArgs);
                }
                Instance.requestsQueue.Clear();
                yield return null;
            }
        }
        public static void RunOnce()
        {
            foreach (var valueTuple in Instance.requestsQueue)
            {
                var (eventTypes, sender, eventArgs) = valueTuple;
                var del = Instance.listOfHandlers[(uint)eventTypes];
                del?.Invoke(sender, eventArgs);
            }
            Instance.requestsQueue.Clear();
        }

        public static async Task RunAsync()
        {
            //TODO: Broken now
            void Function()
            {
                while (true)
                {
                    foreach (var valueTuple in Instance.requestsQueue)
                    {
                        var (eventTypes, sender, eventArgs) = valueTuple;

                        void TryInvokeHandlers()
                        {
                            Instance.listOfHandlers[(uint)eventTypes]?.Invoke(sender, eventArgs);
                        }

                        //TODO: parametrize time
                        var cts = new CancellationTokenSource(2);
                        Task.Run(TryInvokeHandlers, cts.Token);
                    }
                }
            }

            await Task.Run(Function, new CancellationTokenSource(33).Token);
        }

    }


}
