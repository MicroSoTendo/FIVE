using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class HostHandler : NetworkHandler
    {
        private readonly TcpListener listener;
        private readonly HandShaker handShaker;
        private readonly ConcurrentDictionary<int, (TcpClient, InGameHandler)> privateIDToclients 
            = new ConcurrentDictionary<int, (TcpClient, InGameHandler)>();
        private readonly ConcurrentDictionary<int, (TcpClient, InGameHandler)> publicIDToclients 
            = new ConcurrentDictionary<int, (TcpClient, InGameHandler)>();
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
                (int publicID, int privateID) = await handShaker.HandShakeAsync(client);
                var inGameHandler = InGameHandler.CreateHostHandler(client);
                privateIDToclients.TryAdd(privateID, (client, inGameHandler));
                publicIDToclients.TryAdd(publicID, (client, inGameHandler));
                inGameHandler.Start();
            }
        }

    }
}
