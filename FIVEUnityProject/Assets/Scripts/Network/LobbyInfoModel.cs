using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using FIVE.UI;
using Photon.Realtime;

namespace FIVE.Network
{
    public class LobbyInfoModel : Observable
    {
        public ObservableCollection<RoomInfo> RoomsList { get; }

        public LobbyInfoModel()
        {
            RoomsList = new ObservableCollection<RoomInfo>();
            RoomsList.CollectionChanged += OnRoomsListChanged;
        }

        private void OnRoomsListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChange(new PropertyChangedEventArgs(nameof(RoomsList)));
        }

    }

}