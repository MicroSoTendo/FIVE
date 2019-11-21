using System;
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

        protected abstract void DoUpdate();
        protected NetworkStream Stream { get; }
        private readonly ConcurrentQueue<TimeStamp> sendQueue;
        private readonly ConcurrentQueue<TimeStamp> readQueue;
        private CancellationTokenSource cts;
        private Task readTask;
        private Task sendTask;
        protected TimeStamp LocalStamp;
        protected TimeStamp RemoteStamp;

        protected SyncHandler(TcpClient client)
        {
            Stream = client.GetStream();
            sendQueue = new ConcurrentQueue<TimeStamp>();
            readQueue = new ConcurrentQueue<TimeStamp>();
            cts = new CancellationTokenSource();
            readTask = ReadStampAsync(cts.Token);
            sendTask = SendStampAsync(cts.Token);
            LocalStamp = new TimeStamp {Time = 0};
        }

        private async Task SendStampAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                while (sendQueue.TryDequeue(out TimeStamp stamp))
                {
                    await Stream.WriteAsync(stamp.Time.ToBytes(), 0, 4, ct);
                    await Stream.WriteAsync(stamp.Data.Count.ToBytes(), 0, 4, ct);
                    while (stamp.Data.TryDequeue(out byte[] data))
                    {
                        await Stream.WriteAsync(data, 0, data.Length, ct);
                    }
                }
            }
        }


        private async Task ReadStampAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                byte[] timeBuffer = new byte[4];
                await Stream.ReadAsync(timeBuffer, 0, 4, ct);
                byte[] countBuffer = new byte[4];
                await Stream.ReadAsync(countBuffer, 0, 4, ct);
                readQueue.Enqueue(UnsafeReadHelper(timeBuffer, countBuffer));
            }
        }

        private unsafe TimeStamp UnsafeReadHelper(byte[] timeBuffer, byte[] countBuffer)
        {
            var ts = new TimeStamp();
            fixed(byte* pTime = timeBuffer, pCount = countBuffer)
            {
                ts.Time = *(int*)pTime;
                for (int i = 0; i < *(int*)pCount; i++)
                {
                    int bufferSize = Stream.ReadI32();
                    byte[] buffer = new byte[bufferSize];
                    Stream.Read(buffer);
                    ts.Data.Enqueue(buffer);
                }
            }
            return ts;
        }

        protected void Send(byte[] buffer)
        {
            LocalStamp.Data.Enqueue(buffer.Length.ToBytes());
            LocalStamp.Data.Enqueue(buffer);
        }

        protected bool TryRead(out byte[] data)
        {
            data = default;
            return RemoteStamp != null && RemoteStamp.Data.TryDequeue(out data);
        }

        public void LateUpdate()
        {
            LocalStamp = new TimeStamp();
            RemoteStamp = readQueue.TryDequeue(out TimeStamp newStamp) ? newStamp : null;
            DoUpdate();
            sendQueue.Enqueue(LocalStamp);
        }

        public void Stop()
        {
            cts.Cancel();
        }
    }
}
