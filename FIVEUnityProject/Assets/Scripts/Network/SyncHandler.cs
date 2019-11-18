using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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

        public abstract void Update();

        protected NetworkStream Stream { get; }
        
        private readonly ConcurrentQueue<byte[]> sendQueue;
        private readonly ConcurrentQueue<byte[]> readQueue;
        private CancellationTokenSource cts;
        private Task readTask;
        private Task sendTask;
        protected SyncHandler(TcpClient client)
        {
            Stream = client.GetStream();
            sendQueue = new ConcurrentQueue<byte[]>();
            readQueue = new ConcurrentQueue<byte[]>();
            cts = new CancellationTokenSource();
            readTask = ReadAsync(cts.Token);
            sendTask = SendAsync(cts.Token);
        }

        private async Task SendAsync(CancellationToken ct)
        {
            while (true)
            {
                while (sendQueue.TryDequeue(out byte[] buffer))
                {
                    await Stream.WriteAsync(buffer, 0, buffer.Length, ct);
                }
            }
        }

        private async Task ReadAsync(CancellationToken ct)
        {
            while (true)
            {
                byte[] sizeBuffer = new byte[4];
                await Stream.ReadAsync(sizeBuffer, 0, 4, ct);
                readQueue.Enqueue(UnsafeReadHelper(sizeBuffer));
            }
        }

        private unsafe byte[] UnsafeReadHelper(byte[] sizeBuffer)
        {
            fixed (byte* pSizeBuffer = sizeBuffer)
            {
                byte[] buffer = new byte[*(int*)pSizeBuffer];
                Stream.Read(buffer);
                return buffer;
            }
        }

        protected void Send(byte[] buffer)
        {
            sendQueue.Enqueue(buffer.Length.ToBytes());
            sendQueue.Enqueue(buffer);
        }

        protected byte[] Read()
        {
            while (true)
            {
                if (readQueue.TryDequeue(out byte[] next))
                    return next;
            }
        }
    }
}
