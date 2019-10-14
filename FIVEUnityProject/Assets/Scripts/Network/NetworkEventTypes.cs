using FIVE.EventSystem;
using System;
using System.Collections.Generic;

namespace FIVE.Network
{
    public abstract class OnCreateRoomRequested : IEventType<CreateRoomRequestedEventArgs> { }

    public class CreateRoomRequestedEventArgs : EventArgs
    {
        public CreateRoomRequestedEventArgs(RoomInfo roomInfo)
        {
            RoomInfo = roomInfo;
        }
        public RoomInfo RoomInfo { get; }
    }
    public abstract class OnConnected : IEventType { }
    public abstract class OnConnectedToMaster : IEventType { }
    public abstract class OnJoinedLobby : IEventType { }
    public abstract class OnJoinedRoom : IEventType { }

    public class OnRoomListUpdateEventArgs : EventArgs
    {
        public List<object> RoomInfos { get; }
        public OnRoomListUpdateEventArgs(List<object> roomInfos)
        {
            RoomInfos = roomInfos;
        }
    }

    public abstract class OnRoomListUpdate : IEventType<OnRoomListUpdateEventArgs> { }
}
