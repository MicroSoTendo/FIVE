using FIVE.EventSystem;
using System;
using System.Threading.Tasks;

namespace FIVE.UI
{
    public class OnObservableChanged<TEventArgs> : IEventType<TEventArgs> where TEventArgs : EventArgs { }
    public abstract class Observable<TEventType, TEventArgs> 
        where TEventType : OnObservableChanged<TEventArgs>
        where TEventArgs : EventArgs
    {
        public event EventHandler<TEventArgs> OnObservableChanged
        {
            add
            {
                EventManager.Subscribe<TEventType, TEventArgs>(value);
            }

            remove
            {
                EventManager.Unsubscribe<TEventType, TEventArgs>(value);
            }
        }

        protected void RaiseObservableChanged(TEventArgs observableChangedEventArgs)
        {
            this.RaiseEvent<TEventType, TEventArgs>(observableChangedEventArgs);
        }

        protected async Task RaiseObservableChangedAsync(TEventArgs propertyChangedEventArgs)
        {
            await this.RaiseEventAsync<TEventType, TEventArgs>(propertyChangedEventArgs);
        }

    }
}