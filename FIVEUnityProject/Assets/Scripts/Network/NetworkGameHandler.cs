using System;
using System.Threading;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal abstract class NetworkGameHandler : IDisposable
    {
        protected Task HandlerTask;
        protected CancellationTokenSource TokenSource;
        protected event Action OnStart;
        protected event Action OnStop;
        public virtual int UpdateRate { get; set; }
        public virtual bool IsRunning { get; protected set; }

        public virtual void Start()
        {
            if (IsRunning) throw new Exception(GetType() + " is already running.");
            OnStart?.Invoke();
            IsRunning = true;
            TokenSource = new CancellationTokenSource();
            HandlerTask = Task.Run(Handler, TokenSource.Token);
        }

        public virtual void Stop()
        {
            if (!IsRunning) throw new Exception(GetType() + " is not running.");
            OnStop?.Invoke();
            IsRunning = false;
            TokenSource.Cancel();
            HandlerTask.Dispose();
            TokenSource.Dispose();
        }
        
        protected abstract Task Handler();

        public void Dispose()
        {
            HandlerTask?.Dispose();
            TokenSource?.Dispose();
        }
    }
}
