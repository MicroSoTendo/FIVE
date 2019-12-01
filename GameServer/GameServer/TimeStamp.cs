using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class TimeStamp
    {
        public TimeStamp(int time)
        {
            Time = time;
            Stamp = new ConcurrentQueue<byte[]>();
        }

        public int Time { get; }
        public ConcurrentQueue<byte[]> Stamp { get; }
    }
}
