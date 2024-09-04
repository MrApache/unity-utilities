using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Irisu.Collections
{
    [PublicAPI]
    public static class DictionaryExtensions
    {
        public static void ToDictionary<TK, TV>(this List<SerializablePair<TK, TV>> serializablePairs,
            Dictionary<TK, TV> dictionary, bool clearDictionary)
        {
            if(clearDictionary)
                dictionary.Clear();

            foreach ((TK key, TV value) in serializablePairs)
            {
                dictionary[key] = value;
            }
        }

        public static Dictionary<TK, TV> ToDictionary<TK, TV>(this List<SerializablePair<TK, TV>> serializablePairs)
        {
            Dictionary<TK, TV> dictionary = new Dictionary<TK, TV>();
            foreach ((TK key, TV value) in serializablePairs)
            {
                dictionary[key] = value;
            }

            return dictionary;
        }

        public static void ToSerializableList<TK, TV>(this Dictionary<TK, TV> dictionary, List<SerializablePair<TK, TV>> list, bool clearList)
        {
            if(clearList)
                list.Clear();

            foreach ((TK key, TV value) in dictionary)
            {
                list.Add(new SerializablePair<TK, TV>(key, value));
            }
        }

        public static List<SerializablePair<TK, TV>> ToSerializableList<TK, TV>(this Dictionary<TK, TV> dictionary)
        {
            List<SerializablePair<TK, TV>> list = new List<SerializablePair<TK, TV>>();
            foreach ((TK key, TV value) in dictionary)
            {
                list.Add(new SerializablePair<TK, TV>(key, value));
            }

            return list;
        }

        public static SerializablePair<TK, TV> ToSerializablePair<TK, TV>(this KeyValuePair<TK, TV> pair)
        {
            return new SerializablePair<TK, TV>(pair.Key, pair.Value);
        }
    }

    [Serializable]
    public struct SerializablePair<TK, TV>
    {
        [SerializeField] private TK _key;
        [SerializeField] private TV _value;

        public TK Key => _key;
        public TV Value => _value;

        public SerializablePair(TK key, TV value)
        {
            _key = key;
            _value = value;
        }

        public void Deconstruct(out TK key, out TV value)
        {
            key = _key;
            value = _value;
        }

        public override string ToString()
        {
            return $"{Key}-{Value}";
        }
    }
}