using FIVE.EventSystem;
using System;
using System.Threading.Tasks;

namespace FIVE.UI
{
    public abstract class Observable<TEventArgs> where TEventArgs : EventArgs
    {
        public event EventHandler<TEventArgs> OnObservableChanged
        {
            add
            {
                EventManager.Subscribe<OnObservableChanged<TEventArgs>, TEventArgs>(value);
            }

            remove
            {
                EventManager.Unsubscribe<OnObservableChanged<TEventArgs>, TEventArgs>(value);
            }
        }

        protected void RaiseObservableChanged(TEventArgs observableChangedEventArgs)
        {
            this.RaiseEvent<OnObservableChanged<TEventArgs>, TEventArgs>(observableChangedEventArgs);
        }

        protected async Task RaiseObservableChangedAsync(TEventArgs propertyChangedEventArgs)
        {
            await this.RaiseEventAsync<OnObservableChanged<TEventArgs>, TEventArgs>(propertyChangedEventArgs);
        }
    }
}