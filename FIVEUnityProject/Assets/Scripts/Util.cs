using FIVE.EventSystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE
{
    public static class Util
    {
        public static GameObject GetChildGameObject(this GameObject gameObject, string name)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    GameObject childInChild = child.GetChildGameObject(name);
                    if (childInChild != null)
                    {
                        return childInChild;
                    }
                }
            }

            return null;
        }

        public static List<GameObject> GetChildGameObjects(this GameObject gameObject)
        {
            var results = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                results.Add(child);
                results.AddRange(child.GetChildGameObjects());
            }

            return results;
        }

        public static void Subscribe<T>(Action action) where T : IEventType
        {
            EventManager.Subscribe<T>((s, a) => action());
        }        
        
        public static void RaiseEvent<T>(this object sender, EventArgs args = null) where T : IEventType
        {
            EventManager.RaiseEvent<T>(sender, args ?? EventArgs.Empty);
        }

        public static void RaiseEventFixed<T>(this object sender, EventArgs args = null, int millisecondsDelay = 0)
            where T : IEventType
        {
            EventManager.RaiseEventFixed<T>(sender, args ?? EventArgs.Empty, millisecondsDelay);
        }

        public static void RaiseEvent<T, TEventArgs>(this object sender, TEventArgs args)
            where T : IEventType<TEventArgs>
            where TEventArgs : EventArgs
        {
            EventManager.RaiseEvent<T, TEventArgs>(sender, args);
        }

        public static void RaiseEvent<T, THandler, TEventArgs>(this object sender, TEventArgs args)
            where T : IEventType<THandler, TEventArgs>
            where THandler : Delegate
            where TEventArgs : EventArgs
        {
            EventManager.RaiseEvent<T, THandler, TEventArgs>(sender, args);
        }

        public static async Task RaiseEventAsync<T>(this object sender, EventArgs args)
        {
            await EventManager.RaiseEventAsync<T>(sender, args);
        }

        public static async Task RaiseEventAsync<T, TEventArgs>(this object sender, EventArgs args)
        {
            await EventManager.RaiseEventAsync<T>(sender, args);
        }

        public static async Task RaiseEventAsync<T, THandler, TEventArgs>(this object sender, TEventArgs args)
            where T : IEventType<THandler, TEventArgs>
            where THandler : Delegate
            where TEventArgs : EventArgs
        {
            await EventManager.RaiseEventAsync<T, THandler, TEventArgs>(sender, args);
        }
    }
}