using System.Net.Sockets;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class ClientHandler : NetworkHandler
    {
        private readonly TcpClient client;
        private readonly HandShaker handShaker;
        private InGameHandler inGameHandler;
        public ClientHandler(TcpClient client, HandShaker handShaker)
        {
            this.client = client;
            this.handShaker = handShaker;
        }
        protected override async Task Handler()
        {
            (int publicID, int privateID) = await handShaker.HandShakeAsync(client);
            if (publicID != -1)
            {
                NetworkManager.Instance.PlayerIndex = publicID;
                NetworkManager.Instance.PrivateID = privateID;
                NetworkManager.Instance.State = NetworkManager.NetworkState.Client;
                inGameHandler = InGameHandler.CreateClientHandler(client);
                inGameHandler.Start();
            }
        }
    }
}
