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
        private readonly ConcurrentBag<SyncHandler> hostHandlers;
        private CancellationTokenSource cts;
        private Task incomingConnectionTask;
        public HostGameHandler()
        {
            listener = new TcpListener(IPAddress.Any, NetworkManager.Instance.CurrentRoomInfo.Port);
            hostHandlers = new ConcurrentBag<SyncHandler>();
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

        public override void Start()
        {
            listener.Start();
            cts = new CancellationTokenSource();
            incomingConnectionTask = HandleIncomingAsync(cts.Token);
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

        public override void Stop()
        { 
            listener.Stop();
            foreach (SyncHandler hostHandler in hostHandlers)
            {
                hostHandler.Stop();
            }
            cts.Cancel();
        }
        /// <summary>
        /// Late update run from main thread.
        /// </summary>
        public override void LateUpdate()
        {
            foreach (SyncHandler hostHandler in hostHandlers)
            {
                hostHandler.LateUpdate();
            }
        }

        public override void Dispose()
        {
            incomingConnectionTask?.Dispose();
        }
    }
}
