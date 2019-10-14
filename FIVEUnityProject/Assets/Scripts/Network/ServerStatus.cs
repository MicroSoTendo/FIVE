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

        public int LastLatency { get; set; }
        public Ping Ping { get; set; }

        public ServerStatus(string ip, ushort port, string title, ushort players, ushort capacity)
        {
            IP = ip;
            Port = port;
            Title = title;
            Players = players;
            Capacity = capacity;
            Ping = new Ping(ip);
        }
    }
}