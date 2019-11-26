using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public class Worker
    {
        public static ConcurrentDictionary<int, Worker> Workers { get; } = new ConcurrentDictionary<int, Worker>();
        public int Id { get; }
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private bool hasReceived = false;
        private TimeStamp receivedStamp;
        public Worker(int id, TcpClient client)
        {
            Id = id;
            this.client = client;
            stream = client.GetStream();
            Workers.TryAdd(id, this);
        }

        public async Task DoReceiveAsync(CancellationToken ct)
        {
            ushort header = stream.Read<ushort>();
            //TODO: Check header
            int time = stream.Read<int>();
            receivedStamp = new TimeStamp(time);
            int count = stream.Read<int>();
            for (int i = 0; i < count; i++)
            {
                int size = stream.Read<int>();
                byte[] buffer = new byte[size];
                await stream.ReadAsync(buffer, ct);
                receivedStamp.Stamp.Enqueue(buffer);
            }
            hasReceived = true;
        }

        public async Task DoSendAsync(CancellationToken ct)
        {
            if (!hasReceived) return;
            foreach (Worker worker in Workers.Values)
            {
                if (worker == this) continue;
                foreach (byte[] bytes in worker.receivedStamp.Stamp)
                {
                    stream.Write(bytes.Length);
                    await stream.WriteAsync(bytes, ct);
                }
            }
            hasReceived = false;
        }
    }

    public static class WorkerExtension
    {
        public static unsafe T Read<T>(this NetworkStream stream) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];
            stream.Read(buffer);
            fixed (byte* pBuffer = buffer)
            {
                return *(T*)pBuffer;
            }
        }

        public static unsafe void Write<T>(this NetworkStream stream, T value) where T : unmanaged
        {
            byte[] buffer = new byte[sizeof(T)];
            fixed (byte* pBuffer = buffer)
            {
                *(T*)pBuffer = value;
            }
            stream.Write(buffer);
        }
    }
}
