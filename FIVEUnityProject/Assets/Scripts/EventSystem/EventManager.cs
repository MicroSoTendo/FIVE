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
        private readonly ConcurrentDictionary<Type, HandlerList<HandlerNode<EventHandler>>> defaultHandlerNodes;
        private readonly ConcurrentDictionary<Type, HandlerList<HandlerNode>> dynamicHandlerNodes;
        private readonly ConcurrentQueue<Action> scheduledAsync = new ConcurrentQueue<Action>();
        private readonly ConcurrentQueue<Action> scheduledMainThread = new ConcurrentQueue<Action>();
        private readonly int asynchronousTimeOut; //ms
        private readonly MainThreadDispatcher dispatcher;
        private bool initialized;
        private Coroutine coroutine;
        private Task asyncTask;

        private EventManager()
        {
            var mainThreadDispatcther = new GameObject(nameof(MainThreadDispatcher));
            dispatcher = mainThreadDispatcther.AddComponent<MainThreadDispatcher>();
            defaultHandlerNodes = new ConcurrentDictionary<Type, HandlerList<HandlerNode<EventHandler>>>();
            dynamicHandlerNodes = new ConcurrentDictionary<Type, HandlerList<HandlerNode>>();
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
                Instance.defaultHandlerNodes.TryAdd(type, new HandlerList<HandlerNode<EventHandler>>());
                Instance.dynamicHandlerNodes.TryAdd(type, new HandlerList<HandlerNode>());
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
                        if (scheduledMainThread.TryDequeue(out Action action))
                        {
                            action();
                        }

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
                    scheduledAsync.TryDequeue(out Action action);
                    await Task.Run(action, new CancellationTokenSource(asynchronousTimeOut).Token);
                }
                Thread.Sleep(1);
            }
        }

        public static void Subscribe<T>(EventHandler handler, bool requiresMainThread = true) where T : IEventType
        {
            Assert.IsTrue(Instance.initialized);
            Instance.defaultHandlerNodes[typeof(T)].Add(new HandlerNode<EventHandler>(handler, requiresMainThread));
        }

        public static void Subscribe<T, TEventArgs>(EventHandler<TEventArgs> handler, bool requiresMain = true)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            Assert.IsTrue(Instance.initialized);
            Instance.dynamicHandlerNodes[typeof(T)].Add(new HandlerNode(handler, requiresMain));
        }

        public static void Subscribe<T, THandler, TEventArgs>(THandler handler, bool requiresMain = true)
            where T : IEventType<THandler, TEventArgs>
            where THandler : Delegate
            where TEventArgs : EventArgs
        {
            Assert.IsTrue(Instance.initialized);
            Instance.dynamicHandlerNodes[typeof(T)].Add(new HandlerNode(handler, requiresMain));
        }

        public static void Unsubscribe<T>(EventHandler handler, bool requiredMain = true) where T : IEventType
        {
            Assert.IsTrue(Instance.initialized);
            Instance.defaultHandlerNodes[typeof(T)].Remove(new HandlerNode<EventHandler>(handler, requiredMain));
        }

        public static void Unsubscribe<T, TEventArgs>(EventHandler<TEventArgs> handler, bool requiredMain = true)
            where T : IEventType
        {
            Assert.IsTrue(Instance.initialized);
            Instance.dynamicHandlerNodes[typeof(T)].Remove(new HandlerNode<EventHandler<TEventArgs>>(handler, requiredMain));
        }

        public static void Unsubscribe<T, THandler, TEventArgs>(THandler handler, bool requiresMain = true)
            where T : IEventType<THandler, TEventArgs>
            where THandler : Delegate
            where TEventArgs : EventArgs
        {
            Assert.IsTrue(Instance.initialized);
            Instance.dynamicHandlerNodes[typeof(T)].Remove(new HandlerNode(handler, requiresMain));
        }

        public static async Task RaiseEventAsync<T>(object sender, EventArgs args)
        {
            var concreteTypeEventTask = Task.Run(() =>
            {
                foreach ((bool requiresMain, List<HandlerNode<EventHandler>> handlerNodes) in Instance.defaultHandlerNodes[typeof(T)])
                {
                    ConcurrentQueue<Action> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (HandlerNode<EventHandler> handlerNode in handlerNodes)
                    {
                        queue.Enqueue(delegate { handlerNode.Handler(sender, args); });
                    }
                }
            });

            var concreteTypeEventTaskDynamicInvoke = Task.Run(() =>
            {
                foreach ((bool requiresMain, List<HandlerNode> handlerNodes) in Instance.dynamicHandlerNodes[typeof(T)])
                {
                    ConcurrentQueue<Action> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (HandlerNode handlerNode in handlerNodes)
                    {
                        queue.Enqueue(delegate { handlerNode.Handler.DynamicInvoke(sender, args); });
                    }
                }
            });

            var baseTypeEventTask = Task.Run(() =>
            {
                Type baseType = typeof(T).BaseType;
                Debug.Log(baseType.Name);
                if (!Instance.defaultHandlerNodes.ContainsKey(baseType))
                {
                    return;
                }

                foreach ((bool requiresMain, List<HandlerNode<EventHandler>> handlerNodes) in Instance.defaultHandlerNodes[baseType])
                {
                    ConcurrentQueue<Action> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (HandlerNode<EventHandler> handlerNode in handlerNodes)
                    {
                        queue.Enqueue(delegate { handlerNode.Handler(sender, args); });
                    }
                }
            });

            var baseTypeEventTaskDynamicInvoke = Task.Run(() =>
            {
                Type baseType = typeof(T).BaseType;
                Debug.Log(baseType.Name);
                if (!Instance.defaultHandlerNodes.ContainsKey(baseType))
                {
                    return;
                }

                foreach ((bool requiresMain, List<HandlerNode> handlerNodes) in Instance.dynamicHandlerNodes[baseType])
                {
                    ConcurrentQueue<Action> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                    foreach (HandlerNode handlerNode in handlerNodes)
                    {
                        queue.Enqueue(delegate { handlerNode.Handler.DynamicInvoke(sender, args); });
                    }
                }
            });

            await Task.WhenAll(concreteTypeEventTask, baseTypeEventTask, baseTypeEventTaskDynamicInvoke, concreteTypeEventTaskDynamicInvoke);
        }

        public static async Task RaiseEventAsync<T, THandler, TEventArgs>(object sender, TEventArgs args)
            where T : IEventType<THandler, TEventArgs>
            where THandler : Delegate
            where TEventArgs : EventArgs
        {
            await RaiseEventAsync<T>(sender, args);
        }        
        
        
        public static async Task RaiseEventAsync<T, TEventArgs>(object sender, TEventArgs args)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            await RaiseEventAsync<T>(sender, args);
        }

        public static void RaiseEvent<T>(object sender, EventArgs args) where T : IEventType
        {
            foreach ((bool requiresMain, List<HandlerNode<EventHandler>> handlerNodes) in Instance.defaultHandlerNodes[typeof(T)])
            {
                ConcurrentQueue<Action> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                foreach (HandlerNode<EventHandler> handlerNode in handlerNodes)
                {
                    queue.Enqueue(delegate { handlerNode.Handler(sender, args); });
                }
            }
            foreach ((bool requiresMain, List<HandlerNode> handlerNodes) in Instance.dynamicHandlerNodes[typeof(T)])
            {
                ConcurrentQueue<Action> queue = requiresMain ? Instance.scheduledMainThread : Instance.scheduledAsync;
                foreach (HandlerNode handlerNode in handlerNodes)
                {
                    queue.Enqueue(delegate { handlerNode.Handler.DynamicInvoke(sender, args); });
                }
            }
        }

        public static void RaiseEvent<T, TEventArgs>(object sender, TEventArgs args)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            RaiseEvent<T>(sender, args);
        }

        public static void RaiseEvent<T, THandler, TEventArgs>(object sender, TEventArgs args)
            where T : IEventType<THandler, TEventArgs>
            where THandler : Delegate
            where TEventArgs : EventArgs
        {
            RaiseEvent<T>(sender, args);
        }
    }
}