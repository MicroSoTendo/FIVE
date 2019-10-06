using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.EventSystem
{
    public sealed class EventManager
    {
        private static EventManager Instance { get; } = new EventManager();
        private readonly ConcurrentDictionary<Type, HandlerList> handlerNodes;
        private readonly ConcurrentDictionary<Type, Task> timedEventDictionary;
        private readonly ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)> scheduledMainThread;
        private readonly ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)> scheduledAsync;
        private readonly int asynchronousTimeOut; //ms
        private readonly MainThreadDispatcher dispatcher;
        private bool initialized;
        private Coroutine coroutine;
        private Task asyncTask;

        private EventManager()
        {
            scheduledMainThread = new ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)>();
            scheduledAsync = new ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)>();
            GameObject mainThreadDispatcther = new GameObject(nameof(MainThreadDispatcher));
            dispatcher = mainThreadDispatcther.AddComponent<MainThreadDispatcher>();
            handlerNodes = new ConcurrentDictionary<Type, HandlerList>();
            timedEventDictionary = new ConcurrentDictionary<Type, Task>();
            asynchronousTimeOut = 16;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Instance.initialized)
            {
                return;
            }

            IEnumerable<Type> types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      from type in assembly.GetTypes()
                                      where typeof(IEventType).IsAssignableFrom(type)
                                      select type;
            foreach (Type type in types)
            {
                Instance.handlerNodes.TryAdd(type, new HandlerList());
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
                    if (scheduledMainThread.TryDequeue(out (EventHandler handler, object sender, EventArgs args) tuple))
                    {
                        tuple.handler(tuple.sender, tuple.args);
                    }
                    yield return null;
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
                    if (scheduledAsync.TryDequeue(out (EventHandler handler, object sender, EventArgs args) tuple))
                    {
                        await Task.Run(() => { tuple.handler(tuple.sender, tuple.args); }, new CancellationTokenSource(asynchronousTimeOut).Token);
                    }
                }
                Thread.Sleep(1);
            }
        }

        public static void Subscribe<T>(EventHandler handler, bool requiresMainThread = true) where T : IEventType
        {
            Assert.IsTrue(Instance.initialized);
            Instance.handlerNodes[typeof(T)].Add(HandlerNode.Create(handler, requiresMainThread));
        }

        public static void Subscribe<T, TEventArgs>(EventHandler<TEventArgs> handler, bool requiresMain = true)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            Assert.IsTrue(Instance.initialized);
            Instance.handlerNodes[typeof(T)].Add(HandlerNode.Create(handler, requiresMain));
        }

        public static void Unsubscribe<T>(EventHandler handler, bool requiredMain = true) where T : IEventType
        {
            Assert.IsTrue(Instance.initialized);
            Instance.handlerNodes[typeof(T)].Remove(HandlerNode.Create(handler, requiredMain));
        }

        public static void Unsubscribe<T, TEventArgs>(EventHandler<TEventArgs> handler, bool requiredMain = true)
            where T : IEventType
            where TEventArgs : EventArgs
        {
            Assert.IsTrue(Instance.initialized);
            Instance.handlerNodes[typeof(T)].Remove(HandlerNode.Create(handler, requiredMain));
        }

        public static async Task RaiseEventAsync<T>(object sender, EventArgs args)
        {
            Task concreteTypeEventTask = Task.Run(() =>
            {
                foreach ((bool requiresMain, List<HandlerNode> handlerNodes) in Instance.handlerNodes[typeof(T)])
                {
                    ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (HandlerNode handlerNode in handlerNodes)
                    {
                        queue.Enqueue((handlerNode.Handler, sender, args));
                    }
                }
            });

            Task baseTypeEventTask = Task.Run(() =>
            {
                Type baseType = typeof(T).BaseType;
                if (!Instance.handlerNodes.ContainsKey(baseType))
                {
                    return;
                }
                foreach ((bool requiresMain, List<HandlerNode> handlerNodes) in Instance.handlerNodes[baseType])
                {
                    ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)> queue = 
                        requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (HandlerNode handlerNode in handlerNodes)
                    {
                        queue.Enqueue((handlerNode.Handler, sender, args));
                    }
                }
            });

            await Task.WhenAll(concreteTypeEventTask, baseTypeEventTask);
        }

        public static async Task RaiseEventAsync<T, TEventArgs>(object sender, TEventArgs args)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            await RaiseEventAsync<T>(sender, args);
        }

        public static void RaiseEvent<T>(object sender, EventArgs args) where T : IEventType
        {
            RaiseEventInternal<T>(sender, args);
        }

        public static void RaiseEvent<T, TEventArgs>(object sender, TEventArgs args)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            RaiseEvent<T>(sender, args);
        }

        public static void RaiseEventFixed<T>(object sender, EventArgs e, int millisecondsDelay)
        {
            ConcurrentDictionary<Type, Task> lookUp = Instance.timedEventDictionary;
            if (lookUp.TryGetValue(typeof(T), out Task task))
            {
                if (task.IsCompleted)
                {
                    RaiseEventInternal<T>(sender, e);
                    lookUp[typeof(T)] = Task.Run(async () => { await Task.Delay(millisecondsDelay); });
                }
            }
            else
            {
                RaiseEventInternal<T>(sender, e);
                lookUp.TryAdd(typeof(T), Task.Run(async () => { await Task.Delay(millisecondsDelay); }));
            }
        }

        private static void RaiseEventInternal<T>(object sender, EventArgs args)
        {
            foreach ((bool requiresMain, List<HandlerNode> handlerNodes) in Instance.handlerNodes[typeof(T)])
            {
                ConcurrentQueue<(EventHandler handler, object sender, EventArgs args)> queue = 
                    requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                foreach (HandlerNode handlerNode in handlerNodes)
                {
                    queue.Enqueue((handlerNode.Handler, sender, args));
                }
            }
        }
    }
}