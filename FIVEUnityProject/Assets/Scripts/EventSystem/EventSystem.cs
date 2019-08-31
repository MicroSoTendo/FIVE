using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Script.EventSystem
{
    internal sealed class EventSystem
    {
        private readonly EventHandler[] listOfHandlers = new EventHandler[Enum.GetNames(typeof(EventTypes)).Length];
        private readonly List<(EventTypes eventTypes, object sender, EventArgs eventArgs)> requestsList = new List<(EventTypes eventTypes, object sender, EventArgs eventArgs)>();
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
            Instance.requestsList.Add((eventTypes, sender, eventArgs));
        }

        public static void RunOnce()
        {
            foreach (var valueTuple in Instance.requestsList)
            {
                var (eventTypes, sender, eventArgs) = valueTuple;
                var del = Instance.listOfHandlers[(uint)eventTypes];
                del?.Invoke(sender, eventArgs);
            }
        }

        public static async Task RunAsync()
        {
            void Function()
            {
                while (true)
                {
                    foreach (var valueTuple in Instance.requestsList)
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
