using FIVE.EventSystem;
using System;
using System.Collections.Generic;

namespace FIVE.Network
{
    public abstract class OnCreateRoomRequested : IEventType<CreateRoomRequestedEventArgs>
    {
    }

    public class CreateRoomRequestedEventArgs : EventArgs
    {
        public string Name { get; }
        public bool HasPassword { get; }
        public string Password { get; }
        public int MaxPlayer { get; }

        public CreateRoomRequestedEventArgs(string name, bool hasPassword, string password, int maxPlayer)
        {
            Name = name;
            HasPassword = hasPassword;
            Password = password;
            MaxPlayer = maxPlayer;
        }
    }

    public abstract class OnConnected : IEventType
    {
    }

    public abstract class OnConnectedToMaster : IEventType
    {
    }

    public abstract class OnJoinedLobby : IEventType
    {
    }

    public abstract class OnJoinedRoom : IEventType
    {
    }

    public class RoomListUpdateArgs : EventArgs
    {
        public List<object> RoomInfos { get; }

        public RoomListUpdateArgs(List<object> roomInfos)
        {
            RoomInfos = roomInfos;
        }
    }

    public abstract class OnRoomListUpdate : IEventType<RoomListUpdateArgs>
    {
    }
}