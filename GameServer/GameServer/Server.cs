using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class Server
    {
        public ConcurrentDictionary<int, TcpClient> ConnectedClients { get; }
        private HashSet<int> usedInts = new HashSet<int>();
        private TcpListener listener;
        private Task incomingTask;
        private Task workerTask;
        private CancellationTokenSource cts;

        public Server(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
            ConnectedClients = new ConcurrentDictionary<int, TcpClient>();
        }
        

        public void Start()
        {
            cts = new CancellationTokenSource();
            listener.Start();
            incomingTask = Incoming(cts.Token);
            workerTask = RunWorker(cts.Token);
        }

        private async Task Incoming(CancellationToken ct)
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                int playerId = GetNextId();
                ConnectedClients.TryAdd(playerId, client);
                Worker worker = new Worker(playerId, client);
                await Task.Delay(1, ct);
            }
        }

        private async Task RunWorker(CancellationToken ct)
        {
            List<Task> tasks = new List<Task>();
            
            while (true)
            {
                tasks.AddRange(Worker.Workers.Values.Select(worker => worker.ReceiveAsync(ct)));
                await Task.WhenAll(tasks);
                tasks.Clear();
                tasks.AddRange(Worker.Workers.Values.Select(workers => workers.SendAsync(ct)));
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
        }

        private int GetNextId()
        {
            int i = 0;
            while (usedInts.Contains(i))
            {
                i++;
            }
            return i;
        }


        public void Stop()
        {
            cts.Cancel();
        }


    }
}
