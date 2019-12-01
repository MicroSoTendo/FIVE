using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FIVE.Network.Core
{
    internal class GameServerWorker
    {
        private static ConcurrentDictionary<int, GameServerWorker> workers = new ConcurrentDictionary<int, GameServerWorker>();

        private int id;
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private Task sendTask;
        private Task readTask;
        private TimeStamp inStamp;
        private TimeStamp outStamp;
        private int t = 0;
        private CancellationTokenSource cts;
        public GameServerWorker(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
            inStamp = new TimeStamp(t);
            outStamp = new TimeStamp(t);
        }

        public void Start()
        {
            cts = new CancellationTokenSource();
            readTask = ReadAsync(cts.Token);
            sendTask = SendAsync(cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();
            inStamp.Data.Clear();
            outStamp.Data.Clear();
            readTask.Dispose();
            sendTask.Dispose();
        }

        private async Task ReadAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                GameSyncHeader header = stream.Read<GameSyncHeader>();
                await Task.Delay(1, ct);
            }
        }

        
        private async Task SendAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                
            }
        }

        public void LateUpdate()
        {

        }
    }

    internal static class GameServerWorkerExtension
    {
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.TryDequeue(out _))
            {
                
            }
        }
    }
}