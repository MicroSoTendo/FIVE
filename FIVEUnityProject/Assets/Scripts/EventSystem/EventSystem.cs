using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace Assets.Scripts.EventSystem
{
    public sealed class EventSystem
    {
        public enum RunningMode
        {
            Coroutine,
            Update,
            Asynchronous
        }

        private static EventSystem Instance { get; } = new EventSystem();
        private readonly EventHandler[] listOfHandlers;
        private readonly ConcurrentQueue<(EventTypes eventTypes, object sender, EventArgs eventArgs)> requestsQueue;
        private RunningMode runningMode;
        private bool initialized = false;
        private int asynchronousTimeOut;
        public static void Initialize(EventSystemWrapper root, RunningMode mode = RunningMode.Coroutine, int asynchronousTimeOut = 16)
        {
            Assert.IsFalse(Instance.initialized);
            Instance.runningMode = mode;
            switch (mode)
            {
                case RunningMode.Coroutine:
                    root.StartCoroutine(Instance.EventSystemCoroutine());
                    break;
                case RunningMode.Asynchronous:
                    root.OnUpdate += async () =>
                    {
                        await Instance.RunAsync();
                    };
                    root.gameObject.AddComponent<MainThreadEventDispatcher>();
                    Instance.asynchronousTimeOut = asynchronousTimeOut;
                    break;
                case RunningMode.Update:
                    root.OnUpdate += Instance.RunOnce;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Instance.initialized = true;
        }

        private EventSystem()
        {
            listOfHandlers = new EventHandler[Enum.GetNames(typeof(EventTypes)).Length];
            requestsQueue = new ConcurrentQueue<(EventTypes eventTypes, object sender, EventArgs eventArgs)>();
        }

        public static void Subscribe(EventTypes eventTypes, EventHandler del, bool requiresMainThread = true)
        {
            Assert.IsTrue(Instance.initialized);
            switch (Instance.runningMode)
            {
                case RunningMode.Coroutine:
                case RunningMode.Update:
                case RunningMode.Asynchronous when !requiresMainThread:
                    Instance.listOfHandlers[(uint)eventTypes] += del;
                    break;
                case RunningMode.Asynchronous:
                    Instance.listOfHandlers[(uint)eventTypes] += (s, a) => { MainThreadEventDispatcher.ScheduleEvent(del, s, a); };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void Subscribe<T>(EventTypes eventTypes, EventHandler del, bool requiresMainThread = true)
        {
            void DetermineType(object sender, EventArgs eventArgs)
            {
                if (sender is T) del?.Invoke(sender, eventArgs);
            }
            Subscribe(eventTypes, DetermineType, requiresMainThread);
        }

        public static EventHandler GetHandlers(EventTypes eventTypes)
        {
            Assert.IsTrue(Instance.initialized);
            return Instance.listOfHandlers[(int)eventTypes];
        }

        public static void RaiseEvent(EventTypes eventTypes, object sender, EventArgs eventArgs)
        {
            Assert.IsTrue(Instance.initialized);
            Instance.requestsQueue.Enqueue((eventTypes, sender, eventArgs));
        }

        public static void RaiseEventImmediately(EventTypes eventTypes, object sender, EventArgs eventArgs)
        {
            Assert.IsTrue(Instance.initialized);
            Instance.listOfHandlers[(uint)eventTypes]?.Invoke(sender, eventArgs);
        }

        private void RunOnce()
        {
            while (requestsQueue.Count > 0)
            {
                requestsQueue.TryDequeue(out var result);
                RaiseEventImmediately(result.eventTypes, result.sender, result.eventArgs);
            }
        }

        private IEnumerator EventSystemCoroutine()
        {
            while (true)
            {
                RunOnce();
                yield return null;
            }
        }

        private async Task RunAsync()
        {
            var tasks = new Task[requestsQueue.Count];
            var tokenSources = new CancellationTokenSource[requestsQueue.Count];
            var i = 0;
            while (requestsQueue.Count > 0)
            {
                requestsQueue.TryDequeue(out var result);
                var tokenSource = new CancellationTokenSource(asynchronousTimeOut);
                tokenSources[i] = tokenSource;
                tasks[i++] = Task.Run(() =>
                {
                    RaiseEventImmediately(result.eventTypes, result.sender, result.eventArgs);
                }, tokenSource.Token);
            }
            await Task.WhenAll(tasks);
            foreach (var cancellationTokenSource in tokenSources)
            {
                cancellationTokenSource.Dispose();
            }
        }

    }


}
