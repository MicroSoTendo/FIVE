using System;
using FIVE.EventSystem;

namespace FIVE.UI.Multiplayers
{
    public abstract class OnJoinRoomRequested : IEventType { }
    public abstract class OnCreateRoomRequested : IEventType { }

    public class JoinRoomArgs : EventArgs
    {
        public Guid Guid { get; }
        public string Password { get; }

        public JoinRoomArgs(Guid guid, string password)
        {
            Guid = guid;
            Password = password;
        }
    }
    public class CreateRoomArgs : EventArgs
    {
        public CreateRoomArgs(string name, int maxPlayers, bool hasPassword, string password)
        {
            Name = name;
            HasPassword = hasPassword;
            Password = password;
            MaxPlayers = maxPlayers;
        }

        public string Name { get; }
        public int MaxPlayers { get; }
        public bool HasPassword { get; }
        public string Password { get; }

    }
}