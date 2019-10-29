using System;
using System.Security.Cryptography;
using System.Text;

namespace FIVE.Network
{
    public class RoomInfo
    {
        public Guid Guid { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public bool HasPassword { get; set; }
        public int Host { get; set; }
        public ushort Port { get; set; }
        public string Name { get; set; }
        public byte[] HashedPassword { get; private set; }
        public void SetRoomPassword(string newPassword)
        {
            HashedPassword = Md5.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
        }

        private static readonly MD5 Md5 = MD5.Create();
    }
}