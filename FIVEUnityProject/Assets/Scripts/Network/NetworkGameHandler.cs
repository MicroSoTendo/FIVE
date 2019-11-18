using System;
using System.Threading;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal abstract class NetworkGameHandler : IDisposable
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void Update();
        public abstract void Dispose();
    }
}
