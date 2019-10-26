using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FIVE
{
    public class TwoWayMap<TKey, TValue> : IEnumerable<(TKey key, TValue value)>
    {
        public int Count => keyToValue.Count;
        public TValue this[TKey key] => keyToValue[key];
        public TKey this[TValue value] => valueToKey[value];
        private readonly Dictionary<TKey, TValue> keyToValue;
        private readonly Dictionary<TValue, TKey> valueToKey;

        public TwoWayMap()
        {
            keyToValue = new Dictionary<TKey, TValue>();
            valueToKey = new Dictionary<TValue, TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            keyToValue.Add(key, value);
            valueToKey.Add(value, key);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            return keyToValue.TryGetValue(key, out value);
        }

        public bool TryGet(TValue value, out TKey key)
        {
            return valueToKey.TryGetValue(value, out key);
        }

        public bool Remove(TKey key)
        {
            return valueToKey.Remove(keyToValue[key]) && keyToValue.Remove(key);
        }

        public bool Remove(TValue value)
        {
            return keyToValue.Remove(valueToKey[value]) && valueToKey.Remove(value);
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => keyToValue.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => keyToValue.Values;

        public void Clear()
        {
            keyToValue.Clear();
            valueToKey.Clear();
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