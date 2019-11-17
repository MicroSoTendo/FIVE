using System.Net.Sockets;
using System.Threading.Tasks;
using FIVE.Network.InGameHandlers;

namespace FIVE.Network
{
    internal class ClientGameHandler : NetworkGameHandler
    {
        private readonly TcpClient client;
        private readonly HandShaker handShaker;
        private InGameGameHandler inGameGameHandler;
        public ClientGameHandler(TcpClient client, HandShaker handShaker)
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
                NetworkManager.Instance.State = NetworkManager.NetworkState.Client;
                inGameGameHandler = InGameGameHandler.CreateClientHandler(client);
                inGameGameHandler.Start();
            }
        }
    }
}
