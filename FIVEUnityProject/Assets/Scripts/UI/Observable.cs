using System.ComponentModel;
using System.Threading.Tasks;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;

namespace FIVE.UI
{
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                EventManager.Subscribe<OnPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(value);
            }

            remove
            {
                EventManager.Unsubscribe<OnPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(value);
            }
        }

        protected void OnPropertyChange(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.RaiseEvent<OnPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                propertyChangedEventArgs);
        }

        protected async Task OnPropertyChangeAsync(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            await this.RaiseEventAsync<OnPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                propertyChangedEventArgs);
        }
    }
}