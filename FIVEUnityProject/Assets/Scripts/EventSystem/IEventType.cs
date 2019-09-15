using System;

namespace FIVE.EventSystem
{
    public interface IEventType
    {
    }

    public interface IEventType<THandler, TEventArgs> : IEventType
        where THandler : Delegate
        where TEventArgs : EventArgs
    {
    }
}