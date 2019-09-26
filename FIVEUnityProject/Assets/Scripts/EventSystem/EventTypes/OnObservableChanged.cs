using System;

namespace FIVE.EventSystem
{
    public class OnObservableChanged<TEventArgs> : IEventType<TEventArgs> where TEventArgs : EventArgs
    {
    }
}