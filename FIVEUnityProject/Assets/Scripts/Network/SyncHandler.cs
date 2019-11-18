using System.Net.Sockets;

namespace FIVE.Network
{
    public abstract class SyncHandler
    {
        public static SyncHandler StartNewHost(TcpClient tcpClient)
        {
            return new HostSyncHandler(tcpClient);
        }

        public static SyncHandler StartNewClient(TcpClient tcpClient)
        {
            return new ClientSyncHandler(tcpClient);
        }

        public abstract void Run();
        public abstract void Stop();

        protected NetworkStream Stream { get; }

        protected SyncHandler(TcpClient client)
        {
            Stream = client.GetStream();
        }
    }
}
