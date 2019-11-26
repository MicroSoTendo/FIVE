using System.Collections.Concurrent;

namespace FIVE.Network.Core
{
    public class TimeStamp
    {
        public TimeStamp(int time)
        {
            Time = time;
        }

        public int Time { get; }
        public ConcurrentQueue<byte[]> Data { get; set; } = new ConcurrentQueue<byte[]>();
    }
}
