using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;
using FIVE.Network.InGameHandlers;

namespace FIVE.Network
{
    internal class HostGameHandler : NetworkGameHandler
    {
        private readonly TcpListener listener;
        private readonly HandShaker handShaker;
        private readonly ConcurrentDictionary<int, (TcpClient, InGameGameHandler)> privateIDToclients 
            = new ConcurrentDictionary<int, (TcpClient, InGameGameHandler)>();
        private readonly ConcurrentDictionary<int, (TcpClient, InGameGameHandler)> publicIDToclients 
            = new ConcurrentDictionary<int, (TcpClient, InGameGameHandler)>();
        public HostGameHandler(TcpListener listener, HandShaker handShaker)
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
                (int publicID, int privateID) = await handShaker.HandShakeAsync(client);
                var inGameHandler = InGameGameHandler.CreateHostHandler(client);
                privateIDToclients.TryAdd(privateID, (client, inGameHandler));
                publicIDToclients.TryAdd(publicID, (client, inGameHandler));
                inGameHandler.Start();
            }
        }

    }
}
