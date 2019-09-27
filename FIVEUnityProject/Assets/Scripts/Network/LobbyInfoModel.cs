using Photon.Realtime;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FIVE.Network
{
    public class LobbyInfoModel //: Observable
    {
        public ObservableCollection<RoomInfo> RoomsList { get; }
        public string PlayerName
        {
            get => playerName;
            set => playerName = value;//OnPropertyChange(new PropertyChangedEventArgs(nameof(PlayerName)));
        }

        public bool IsConnecting
        {
            get => isConnecting;
            set => isConnecting = value;//OnPropertyChange(new PropertyChangedEventArgs(nameof(IsConnecting)));
        }
        public bool IsConnected
        {
            get => isConnected;
            set => isConnected = value; //OnPropertyChange(new PropertyChangedEventArgs(nameof(IsConnected)));
        }

        private bool isConnecting;
        private bool isConnected;
        private string playerName;


        public LobbyInfoModel()
        {
            RoomsList = new ObservableCollection<RoomInfo>();
            //RoomsList.CollectionChanged += OnRoomsListChanged;
        }

        private void OnRoomsListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChange(new PropertyChangedEventArgs(nameof(RoomsList)));
        }

    }

}