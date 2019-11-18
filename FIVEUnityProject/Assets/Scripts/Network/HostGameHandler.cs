using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class HostGameHandler : NetworkGameHandler
    {
        private readonly TcpListener listener;
        private CancellationTokenSource cts;
        private Task incomingConnectionTask;
        private ConcurrentBag<SyncHandler> hostHandlers;
        public HostGameHandler()
        {
            listener = new TcpListener(IPAddress.Any, NetworkManager.Instance.RoomInfo.Port);
            Handshaker.HostHandshaker.OnHandshakeSuccess += OnHandshakeSuccess;
            Handshaker.HostHandshaker.OnHandshakeFail += OnHandshakeFail;
        }


        private void OnHandshakeSuccess(TcpClient tcpClient)
        {
            hostHandlers.Add(SyncHandler.StartNewHost(tcpClient));
        }

        private void OnHandshakeFail(TcpClient client)
        {
            //TODO: Close client
        }


        /// <summary>
        /// Used by <b>Host only</b>.<br/>
        /// Handles incoming clients.
        /// </summary>
        private async Task HandleIncomingAsync(CancellationToken ct)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }
                await Handshaker.HostHandshaker.HandShakeAsync(await listener.AcceptTcpClientAsync(), ct);
            }
        }

        public override void Start()
        {
            listener.Start();
            cts = new CancellationTokenSource();
            incomingConnectionTask = HandleIncomingAsync(cts.Token);
        }

        public override void Stop()
        {
            //TODO: Stop all
        }

        public override void Dispose()
        {
            incomingConnectionTask?.Dispose();
        }
    }
}
