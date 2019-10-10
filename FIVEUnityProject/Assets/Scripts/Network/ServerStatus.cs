using UnityEngine;

namespace FIVE.Network
{
    public class ServerStatus
    {
        public string IP { get; set; }
        public ushort Port { get; set; }
        public string Title { get; set; }
        public ushort Players { get; set; }
        public ushort Capacity { get; set; }

        public int lastLatency { get; set; }
        public Ping ping { get; set; }

        public ServerStatus(string ip, ushort port, string title, ushort players, ushort capacity)
        {
            this.IP = ip;
            this.Port = port;
            this.Title = title;
            this.Players = players;
            this.Capacity = capacity;
            ping = new Ping(ip);
        }
    }
}
