using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using UnityEngine;
using EventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using ActionType = System.Collections.Specialized.NotifyCollectionChangedAction;

namespace Irisu.Collections
{
    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary, IDeserializationCallback, ISerializable, ISerializationCallbackReceiver, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        [SerializeField] private List<SerializablePair<TKey, TValue>> _serializedPairs;
        private readonly Dictionary<TKey, TValue> _dictionary;

        public Dictionary<TKey, TValue>.KeyCollection Keys => _dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _dictionary.Values;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;
        public int Count => _dictionary.Count;

        bool IDictionary.IsFixedSize => false;
        bool IDictionary.IsReadOnly => false;
        ICollection IDictionary.Values => _dictionary.Values;
        ICollection IDictionary.Keys => _dictionary.Keys;

        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => _dictionary.Count;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        int ICollection<KeyValuePair<TKey, TValue>>.Count => _dictionary.Count;
        bool ICollection.IsSynchronized => (_dictionary as ICollection).IsSynchronized;
        object ICollection.SyncRoot => (_dictionary as ICollection).SyncRoot;

        object IDictionary.this[object key]
        {
            get => _dictionary[(TKey)key]!;
            set => _dictionary[(TKey)key] = (TValue)value;
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public SerializableDictionary()
        {
            _serializedPairs = new List<SerializablePair<TKey, TValue>>();
            _dictionary = new Dictionary<TKey, TValue>();
        }

        #region Impl
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
            CollectionChanged?.Invoke(this,
                new EventArgs(ActionType.Reset));
        }

        public bool Remove(TKey key)
        {
            if (!_dictionary.Remove(key))
                return false;
            CollectionChanged?.Invoke(this,
                new EventArgs(ActionType.Remove, new[] { key }));
            return true;
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            CollectionChanged?.Invoke(this,
                new EventArgs(ActionType.Add, new[] { key }));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void OnDeserialization(object sender)
        {
            _dictionary.OnDeserialization(sender);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _dictionary.GetObjectData(info, context);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void OnBeforeSerialize()
        {
            _dictionary.ToSerializableList(_serializedPairs, true);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _serializedPairs.ToDictionary(_dictionary, true);
        }
        #endregion

        #region IDictionary

        bool IDictionary.Contains(object key)
        {
            if (key is not TKey tKey)
                return false;
            return ContainsKey(tKey);
        }

        void IDictionary.Remove(object key)
        {
            if (key is not TKey tKey)
                return;
            Remove(tKey);
        }

        void IDictionary.Add(object key, object value)
        {
            if (key is not TKey tKey)
                throw new InvalidCastException(nameof(key));
            if(value is not TValue tValue)
                throw new InvalidCastException(nameof(value));
            Add(tKey, tValue);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return ContainsKey(key);
        }

        #endregion

        #region ICollection
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> pair)
        {
            return ContainsKey(pair.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (_dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> pair)
        {
            return Remove(pair.Key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            (_dictionary as ICollection).CopyTo(array, index);
        }
        #endregion

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}