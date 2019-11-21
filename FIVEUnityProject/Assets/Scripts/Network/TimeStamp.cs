using System.Collections.Concurrent;

namespace FIVE.Network
{
    public class TimeStamp
    {
        public int Time { get; set; }
        public ConcurrentQueue<byte[]> Data { get; set; } = new ConcurrentQueue<byte[]>();
    }
}
