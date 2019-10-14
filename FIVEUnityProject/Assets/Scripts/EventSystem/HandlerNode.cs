using System;

namespace FIVE.EventSystem
{
    public struct HandlerNode : IEquatable<HandlerNode>
    {
        public EventHandler Handler { get; }
        public bool RequiresMainThread { get; }
        private readonly Delegate rawHandler;

        private HandlerNode(EventHandler handler, Delegate rawHandler, bool requiresMainThread)
        {
            this.Handler = handler;
            this.rawHandler = rawHandler;
            this.RequiresMainThread = requiresMainThread;
        }

        public static HandlerNode Create(EventHandler handler, bool requiresMainThread)
        {
            return new HandlerNode(handler, handler, requiresMainThread);
        }

        public static HandlerNode Create<T>(EventHandler<T> handler, bool requiresMainThread) where T : EventArgs
        {
            return new HandlerNode((o, e) => { handler(o, (T)e); }, handler, requiresMainThread);
        }

        public bool Equals(HandlerNode other)
        {
            return Equals(rawHandler, other.rawHandler) && RequiresMainThread == other.RequiresMainThread;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is HandlerNode node)
            {
                return Equals(node);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((rawHandler != null ? rawHandler.GetHashCode() : 0) * 397) ^ RequiresMainThread.GetHashCode();
            }
        }
    }
}