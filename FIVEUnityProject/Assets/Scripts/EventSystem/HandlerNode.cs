using System;

namespace FIVE.EventSystem
{
    public class HandlerNode : IEquatable<HandlerNode>
    {
        public EventHandler Handler { get; }
        public bool RequiresMainThread { get; }

        public HandlerNode(EventHandler handler, bool requiresMainThread)
        {
            Handler = handler;
            RequiresMainThread = requiresMainThread;
        }

        public bool Equals(HandlerNode other)
        {
            return Handler.Equals(other.Handler) && RequiresMainThread == other.RequiresMainThread;
        }

        public override int GetHashCode()
        {
            return Handler.GetHashCode() ^ RequiresMainThread.GetHashCode();
        }
    }
}