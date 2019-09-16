using System.Collections.Generic;

namespace FIVE.EventSystem
{
    public class HandlerList<T> where T : HandlerNode
    {
        private readonly List<(bool requiresMain, List<T> handlers)> storage;
        private readonly object syncroot = new object();

        public HandlerList()
        {
            storage = new List<(bool requiresMain, List<T> handlers)>();
        }

        public HandlerList(IEnumerable<(bool requiresMain, List<T> handlers)> collection)
        {
            storage = new List<(bool requiresMain, List<T> handlers)>(collection);
        }

        public HandlerList(int capacity)
        {
            storage = new List<(bool requiresMain, List<T> handlers)>(capacity);
        }

        public int Count => storage.Count;

        public void Add(T item)
        {
            lock (syncroot)
            {
                if (storage.Count != 0 && item.RequiresMainThread == storage[storage.Count - 1].requiresMain)
                {
                    storage[storage.Count - 1].handlers.Add(item);
                }
                else
                {
                    storage.Add((item.RequiresMainThread, new List<T>() { item }));
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

        public IEnumerator<(bool requiresMain, List<T> handlers)> GetEnumerator()
        {
            lock (syncroot)
            {
                return storage.GetEnumerator();
            }
        }

        public bool Remove(T item)
        {
            lock (syncroot)
            {
                foreach ((bool requiresMain, List<T> handlers) in storage)
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