using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class HostHandler : NetworkHandler
    {
        private readonly TcpListener listener;
        private readonly HandShaker handShaker;
        private readonly ConcurrentDictionary<int, TcpClient> clients = new ConcurrentDictionary<int, TcpClient>();
        public HostHandler(TcpListener listener, HandShaker handShaker)
        {
            this.listener = listener;
            this.handShaker = handShaker;
        }


        /// <summary>
        /// Used by <b>Host only</b>.<br/>
        /// Handles incoming clients.
        /// </summary>
        protected override async Task Handler()
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                int clientID = await handShaker.HandShakeAsync(client);
                clients.TryAdd(clientID, client);
                //TODO: Fix above async
                //TOOD: Start ingame handler
            }
        }

    }
}
