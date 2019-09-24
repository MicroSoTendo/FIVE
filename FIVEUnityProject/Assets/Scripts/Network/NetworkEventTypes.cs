using System;
using System.Collections.Generic;
using FIVE.EventSystem;
using Photon.Realtime;

namespace FIVE.Network
{
    public abstract class OnConnected : IEventType { }
    public abstract class OnConnectedToMaster : IEventType { }
    public abstract class OnJoinedLobby : IEventType { }
    public abstract class OnJoinedRoom : IEventType { }

    public class OnRoomListUpdateEventArgs : EventArgs
    {
        public List<RoomInfo> RoomInfos { get; }
        public OnRoomListUpdateEventArgs(List<RoomInfo> roomInfos) => RoomInfos = roomInfos;
    }

    public abstract class OnRoomListUpdate : IEventType<OnRoomListUpdateEventArgs> { }
}
