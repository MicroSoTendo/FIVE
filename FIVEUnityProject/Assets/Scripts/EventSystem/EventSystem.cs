using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.EventSystem
{
    public sealed class EventSystem
    {
        private static EventSystem Instance { get; } = new EventSystem();
        private readonly ConcurrentDictionary<Type, HandlerList<HandlerNode>> typedHandlerNodes;
        private readonly ConcurrentQueue<Action> scheduledAsync = new ConcurrentQueue<Action>();
        private readonly ConcurrentQueue<Action> scheduledMainThread = new ConcurrentQueue<Action>();
        private readonly int asynchronousTimeOut; //ms
        private readonly MainThreadDispatcher dispatcher;
        private bool initialized;
        private Coroutine coroutine;
        private Task asyncTask;
        private EventSystem()
        {
            var mainThreadDispatcther = new GameObject(nameof(MainThreadDispatcher));
            dispatcher = mainThreadDispatcther.AddComponent<MainThreadDispatcher>();
            typedHandlerNodes = new ConcurrentDictionary<Type, HandlerList<HandlerNode>>();
            asynchronousTimeOut = 16;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Instance.initialized) return;
            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where typeof(IEventType).IsAssignableFrom(type)
                        select type;
            foreach (var type in types)
            {
                Instance.typedHandlerNodes.TryAdd(type, new HandlerList<HandlerNode>());
            }
            Instance.coroutine = Instance.dispatcher.StartCoroutine(Instance.MainThreadConsumer());
            Instance.asyncTask = Task.Run(Instance.AsyncConsumer);
            Instance.initialized = true;
        }

        private IEnumerator MainThreadConsumer()
        {
            while (true)
            {
                while (scheduledMainThread.Count > 0)
                {
                    float countDown = 1f;
                    while (countDown > 0)
                    {
                        if (scheduledMainThread.TryDequeue(out var action))
                            action();
                        yield return null;
                        countDown -= Time.deltaTime;
                    }
                }
                yield return null;
            }
        }

        private async Task AsyncConsumer()
        {
            Thread.CurrentThread.Name = nameof(AsyncConsumer);
            while (true)
            {
                while (scheduledAsync.Count > 0)
                {
                    scheduledAsync.TryDequeue(out var action);
                    await Task.Run(action, new CancellationTokenSource(asynchronousTimeOut).Token);
                }
                Thread.Sleep(1);
            }
        }

        public static void Subscribe<T>(EventHandler handler, bool requiresMainThread = true) where T : IEventType
        {
            Assert.IsTrue(Instance.initialized);
            Instance.typedHandlerNodes[typeof(T)].Add(new HandlerNode(handler, requiresMainThread));
        }

        public static void Unsubscribe<T>(EventHandler handler, bool requiredMain)
        {
            Assert.IsTrue(Instance.initialized);
            Instance.typedHandlerNodes[typeof(T)].Remove(new HandlerNode(handler, requiredMain));
        }

        public static async Task RaiseEventAsync<T>(object sender, EventArgs args)
        {
            await Task.Run(() =>
            {
                foreach (var (requiresMain, handlerNodes) in Instance.typedHandlerNodes[typeof(T)])
                {
                    var queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (var handlerNode in handlerNodes)
                    {
                        queue.Enqueue(delegate { handlerNode.Handler(sender, args); });
                    }
                }
            });
        }

        public static void RaiseEvent<T>(object sender, EventArgs args)
        {
            foreach (var (requiresMain, handlerNodes) in Instance.typedHandlerNodes[typeof(T)])
            {
                var queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                foreach (var handlerNode in handlerNodes)
                {
                    queue.Enqueue(delegate { handlerNode.Handler(sender, args); });
                }
            }
        }




    }


}
