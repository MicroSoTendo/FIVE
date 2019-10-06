using System.Collections.Generic;

namespace FIVE.EventSystem
{
    public class HandlerList
    {
        private readonly List<(bool requiresMain, List<HandlerNode> handlers)> storage;
        private readonly object syncroot = new object();

        public HandlerList()
        {
            storage = new List<(bool requiresMain, List<HandlerNode> handlers)>();
        }

        public HandlerList(IEnumerable<(bool requiresMain, List<HandlerNode> handlers)> collection)
        {
            storage = new List<(bool requiresMain, List<HandlerNode> handlers)>(collection);
        }

        public HandlerList(int capacity)
        {
            storage = new List<(bool requiresMain, List<HandlerNode> handlers)>(capacity);
        }

        public int Count => storage.Count;

        public void Add(HandlerNode item)
        {
            lock (syncroot)
            {
                if (storage.Count != 0 && item.RequiresMainThread == storage[storage.Count - 1].requiresMain)
                {
                    storage[storage.Count - 1].handlers.Add(item);
                }
                else
                {
                    storage.Add((item.RequiresMainThread, new List<HandlerNode>() { item }));
                }
            }
        }

        public void Clear()
        {
            lock (syncroot)
            {
                storage.Clear();
            }
        }

        public IEnumerator<(bool requiresMain, List<HandlerNode> handlers)> GetEnumerator()
        {
            lock (syncroot)
            {
                return storage.GetEnumerator();
            }
        }

        public bool Remove(HandlerNode item)
        {
            lock (syncroot)
            {
                foreach ((bool requiresMain, List<HandlerNode> handlers) in storage)
                {
                    if (handlers.Remove(item))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}