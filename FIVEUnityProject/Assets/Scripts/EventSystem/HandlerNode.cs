using System;

namespace FIVE.EventSystem
{
    public class HandlerNode : IEquatable<HandlerNode>
    {
        public Delegate Handler { get; protected set; }
        public bool RequiresMainThread { get; protected set; }

        public HandlerNode(Delegate handler, bool requiresMainThread)
        {
            this.Handler = handler;
            this.RequiresMainThread = requiresMainThread;
        }

        public bool Equals(HandlerNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Handler, other.Handler) && RequiresMainThread == other.RequiresMainThread;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HandlerNode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Handler != null ? Handler.GetHashCode() : 0) * 397) ^ RequiresMainThread.GetHashCode();
            }
        }
    }

    public class HandlerNode<T> : HandlerNode where T : Delegate
    {
        public new T Handler { get; }

        public HandlerNode(T handler, bool requiresMainThread) : base(handler, requiresMainThread)
        {
            Handler = handler;
            RequiresMainThread = requiresMainThread;
        }
    }
}