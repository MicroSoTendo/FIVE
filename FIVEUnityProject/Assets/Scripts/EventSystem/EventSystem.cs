using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.EventSystem
{
    public sealed class EventSystem
    {
        public enum RunningMode
        {
            Inactive,
            Coroutine,
            Update,
            Asynchronous
        }

        public struct Recipient
        {
            public EventTypes EventTypes { get; }
            public EventHandler Handler { get; }

            public Recipient(EventTypes eventTypes, EventHandler handler)
            {
                Handler = handler;
                EventTypes = eventTypes;
            }
        }

        private static EventSystem Instance { get; } = new EventSystem();
        private readonly EventHandler[] listOfHandlers;
        private readonly ConcurrentQueue<Action> requestsQueue;
        private readonly int asynchronousTimeOut; //ms
        private readonly MainThreadDispatcher dispatcher;
        private RunningMode runningMode;
        private bool initialized;
        private Coroutine coroutine;

        private EventSystem()
        {
            listOfHandlers = new EventHandler[Enum.GetNames(typeof(EventTypes)).Length];
            for (var i = 0; i < listOfHandlers.Length; i++)
            {
                listOfHandlers[i] += (s, e) => { };
            }
            requestsQueue = new ConcurrentQueue<Action>();
            var mainThreadDispatcther = new GameObject(nameof(MainThreadDispatcher));
            dispatcher = mainThreadDispatcther.AddComponent<MainThreadDispatcher>();
            runningMode = RunningMode.Inactive;
            asynchronousTimeOut = 16;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Instance.initialized) return;
            SetRunningMode(RunningMode.Coroutine); //Default mode = Coroutine
            Instance.initialized = true;
        }

        public static void SetRunningMode(RunningMode newMode)
        {
            var oldMode = Instance.runningMode;
            if (oldMode == newMode) return;
            if (oldMode == RunningMode.Coroutine) Instance.dispatcher.StopCoroutine(Instance.coroutine);
            else Instance.dispatcher.OnUpdate = () => { };
            switch (newMode)
            {
                case RunningMode.Coroutine:
                    Instance.coroutine = Instance.dispatcher.StartCoroutine(Instance.EventSystemCoroutine());
                    break;
                case RunningMode.Asynchronous:
                    Instance.dispatcher.OnUpdate = async () => { await Instance.RunAsync(); };
                    break;
                case RunningMode.Update:
                    Instance.dispatcher.OnUpdate = Instance.RunOnce;
                    break;
                case RunningMode.Inactive:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
            }
            Instance.runningMode = newMode;
        }

        public static Recipient Subscribe(EventTypes eventTypes, EventHandler handler, bool requiresMainThread = true)
        {
            Assert.IsTrue(Instance.initialized);
            switch (Instance.runningMode)
            {
                case RunningMode.Inactive:
                case RunningMode.Coroutine:
                case RunningMode.Update:
                case RunningMode.Asynchronous when !requiresMainThread:
                    Instance.listOfHandlers[(uint)eventTypes] += handler;
                    return new Recipient(eventTypes, handler);
                case RunningMode.Asynchronous:
                    void WrappedHandler(object s, EventArgs a)
                    {
                        MainThreadDispatcher.Schedule(delegate { handler(s, a); });
                    }
                    Instance.listOfHandlers[(uint)eventTypes] += WrappedHandler;
                    return new Recipient(eventTypes, WrappedHandler);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Recipient Subscribe<T>(EventTypes eventTypes, EventHandler handler, bool requiresMainThread = true)
        {
            void DetermineType(object sender, EventArgs eventArgs)
            {
                if (sender is T) handler?.Invoke(sender, eventArgs);
            }
            return Subscribe(eventTypes, DetermineType, requiresMainThread);
        }

        public static void Unsubscribe(Recipient recipient)
        {
            Assert.IsTrue(Instance.initialized);
            Instance.listOfHandlers[(uint)recipient.EventTypes] -= recipient.Handler;
        }

        public static EventHandler GetHandlers(EventTypes eventTypes)
        {
            Assert.IsTrue(Instance.initialized);
            return Instance.listOfHandlers[(int)eventTypes];
        }

        public static void RaiseEvent(EventTypes eventTypes, object sender, EventArgs eventArgs)
        {
            Assert.IsTrue(Instance.initialized);
            Debug.Log(nameof(RaiseEvent) + eventTypes + sender);
            Instance.requestsQueue.Enqueue(delegate { Instance.listOfHandlers[(uint) eventTypes](sender, eventArgs); });
        }

        private void RunOnce()
        {
            while (requestsQueue.Count > 0)
            {
                requestsQueue.TryDequeue(out var action);
                action.Invoke();
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
                requestsQueue.TryDequeue(out var action);
                var tokenSource = new CancellationTokenSource(asynchronousTimeOut);
                tokenSources[i] = tokenSource;
                tasks[i++] = Task.Run(action, tokenSource.Token);
            }
            await Task.WhenAll(tasks);
            foreach (var cancellationTokenSource in tokenSources)
            {
                cancellationTokenSource.Dispose();
            }
        }

    }


}
