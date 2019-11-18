using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class ClientGameHandler : NetworkGameHandler
    {
        private readonly TcpClient client;
        private CancellationTokenSource cts;
        private Task handshakeTask;
        private SyncHandler clientHandler;
        public ClientGameHandler()
        {
            client = new TcpClient();
            Handshaker.ClientHandshaker.OnHandshakeSuccess += OnHandshakeSuccess;
            Handshaker.ClientHandshaker.OnHandshakeFail += OnHandshakeFail;
        }
        private void OnHandshakeSuccess(TcpClient tcpClient)
        {
            clientHandler = SyncHandler.StartNewClient(tcpClient);
            NetworkManager.Instance.State = NetworkManager.NetworkState.Client;
        }

        private void OnHandshakeFail(TcpClient c)
        {
            //TODO: Close client
        }


        public override void Start()
        {
            int ip = NetworkManager.Instance.RoomInfo.Host;
            ushort port = NetworkManager.Instance.RoomInfo.Port;
            client.Connect(new IPAddress(ip), port);
            cts = new CancellationTokenSource();
            handshakeTask = Handshaker.ClientHandshaker.HandShakeAsync(client, cts.Token);
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            clientHandler.Update();
        }

        public override void Dispose()
        {
            handshakeTask?.Dispose();
        }
    }
}
