using System.Net.Sockets;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class ClientHandler : NetworkHandler
    {
        private readonly TcpClient client;
        private readonly HandShaker handShaker;
        private InGameHandler inGameHandler;
        /// <summary>
        /// Client ID fetched from host after connected successfully.
        /// </summary>
        public int AssignedClientID { get; private set; }
        public ClientHandler(TcpClient client, HandShaker handShaker)
        {
            this.client = client;
            this.handShaker = handShaker;
        }
        protected override async Task Handler()
        {
            int clientID = await handShaker.HandShakeAsync(client);
            if (clientID != -1)
            {
                AssignedClientID = clientID;
                inGameHandler = InGameHandler.CreateClient(client);
                inGameHandler.Start();
            }
        }
    }
}
