using System.Net.Sockets;

namespace FIVE.Network
{
    public class ClientSyncHandler : SyncHandler
    {
        public ClientSyncHandler(TcpClient tcpClient) : base(tcpClient)
        {

        }
        public override void Run()
        {
            throw new System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}