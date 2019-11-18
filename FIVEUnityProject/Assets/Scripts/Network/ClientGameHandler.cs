using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class ClientGameHandler : NetworkGameHandler
    {
        private readonly TcpClient client;
        private IPEndPoint ipEndPoint;
        private byte[] hashedPassword;
        public ClientGameHandler()
        {
            client = new TcpClient();
        }

        public override void Start()
        {
            int ip = NetworkManager.Instance.RoomInfo.Host;
            ushort port = NetworkManager.Instance.RoomInfo.Port;
            client.Connect(new IPAddress(ip), port);
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
