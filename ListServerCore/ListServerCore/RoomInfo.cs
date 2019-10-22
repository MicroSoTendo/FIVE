using System;

namespace ListServerCore
{
    public struct RoomInfo
    {
        public Guid Guid { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public bool HasPassword { get; set; }
        public int Host { get; set; }
        public ushort Port { get; set; }    
        public string Name { get; set; }

        public RoomInfo(Guid guid, int currentPlayers, int maxPlayers, bool hasPassword, int host, ushort port, string name)
        {
            Guid = guid;
            CurrentPlayers = currentPlayers;
            MaxPlayers = maxPlayers;
            HasPassword = hasPassword;
            Host = host;
            Port = port;
            Name = name;
        }
    }
}
