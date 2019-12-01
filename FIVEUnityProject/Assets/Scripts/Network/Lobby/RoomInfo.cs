using System;

namespace FIVE.Network.Lobby
{
    internal class RoomInfo
    {
        public Guid Guid { get; set; }
        public long Host { get; set; }
        public ushort Port { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public bool HasPassword { get; set; }
        public string Name { get; set; }
        public byte[] HashedPassword { get; set; }
    }
}