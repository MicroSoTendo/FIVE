using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE.Network.Core
{
    internal sealed class GameServer : MonoBehaviour
    {
        public int UpdateRate { get; set; } = 30;
        public ushort ListeningPort { get; set; } = 8888;
        private TcpListener listener;
        private CancellationTokenSource cts;
        private Task incomingTask;
        private ConcurrentDictionary<TcpClient, GameServerWorker> connectedClients;
        public void Awake()
        {
            enabled = false;
            listener = new TcpListener(IPAddress.Any, ListeningPort);
        }

        private void OnEnable()
        {
            cts = new CancellationTokenSource();
            listener.Start();
            connectedClients = new ConcurrentDictionary<TcpClient, GameServerWorker>();
            incomingTask = IncomingAsync(cts.Token);
        }

        private void OnDisable()
        {
            cts.Cancel();
            listener.Stop();
        }

        private async Task IncomingAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                TcpClient incomingClient = await listener.AcceptTcpClientAsync();
                connectedClients.TryAdd(incomingClient, new GameServerWorker(incomingClient));
            }
        }


        public void LateUpdate()
        {
            foreach (GameServerWorker worker in connectedClients.Values)
            {
                worker.LateUpdate();
            }
        }
    }
}
