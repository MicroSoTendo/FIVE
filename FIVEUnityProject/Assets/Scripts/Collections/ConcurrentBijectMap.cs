using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FIVE
{
    public class ConcurrentBijectMap<TKey, TValue> : IEnumerable<(TKey key, TValue value)>
    {
        public int Count => keyToValue.Count;
        public TValue this[TKey key] => keyToValue[key];
        public TKey this[TValue value] => valueToKey[value];
        private readonly Dictionary<TKey, TValue> keyToValue;
        private readonly Dictionary<TValue, TKey> valueToKey;

        private readonly object syncRoot;
        public ConcurrentBijectMap()
        {
            syncRoot = new object();
            keyToValue = new Dictionary<TKey, TValue>();
            valueToKey = new Dictionary<TValue, TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            lock (syncRoot)
            {
                keyToValue.Add(key, value);
                valueToKey.Add(value, key);
            }
        }

        public void Add(TValue value, TKey key)
        {
            lock (syncRoot)
            {
                keyToValue.Add(key, value);
                valueToKey.Add(value, key);
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            bool result;
            lock (syncRoot)
            {
                result = keyToValue.TryGetValue(key, out value);
            }
            return result;
        }

        public bool TryGet(TValue value, out TKey key)
        {
            bool result;
            lock (syncRoot)
            {
                result = valueToKey.TryGetValue(value, out key);
            }
            return result;
        }

        public bool Remove(TKey key)
        {
            bool result;
            lock (syncRoot)
            {
                result = valueToKey.Remove(keyToValue[key]) && keyToValue.Remove(key);
            }

            return result;
        }

        public bool Remove(TValue value)
        {
            bool result;
            lock (syncRoot)
            {
                result = keyToValue.Remove(valueToKey[value]) && valueToKey.Remove(value);
            }

            return result;
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => keyToValue.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => keyToValue.Values;

        public void Clear()
        {
            lock (syncRoot)
            {
                keyToValue.Clear();
                valueToKey.Clear();
            }
        }

        public bool Contains(TKey key)
        {
            return keyToValue.ContainsKey(key);
        }

        public bool Contains(TValue value)
        {
            return valueToKey.ContainsKey(value);
        }

        public IEnumerator<(TKey, TValue)> GetEnumerator()
        {
            return keyToValue.Select(keyValuePair => (keyValuePair.Key, keyValuePair.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}