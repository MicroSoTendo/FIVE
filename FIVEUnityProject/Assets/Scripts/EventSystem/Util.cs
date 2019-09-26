using System;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE.EventSystem
{
    public static class Util
    {
        public static void RaiseEvent<T>(this object sender, EventArgs args = null) where T : IEventType
        {
            EventManager.RaiseEvent<T>(sender, args ?? EventArgs.Empty);
            Debug.Log($"{sender} raised event {typeof(T)}");
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