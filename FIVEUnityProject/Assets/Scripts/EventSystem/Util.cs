using System;
using System.Threading.Tasks;

namespace FIVE.EventSystem
{
    public static class Util
    {
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

        public static async Task RaiseEventAsync<T>(this object sender, EventArgs args)
        {
            await EventManager.RaiseEventAsync<T>(sender, args);
        }

        public static async Task RaiseEventAsync<T, TEventArgs>(this object sender, EventArgs args)
        {
            await EventManager.RaiseEventAsync<T>(sender, args);
        }

    }
}