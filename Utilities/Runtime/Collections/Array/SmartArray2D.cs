using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
#nullable enable

namespace Irisu.Collections
{
    [PublicAPI]
    public sealed class SmartArray2D<T> : IList<T>, IReadOnlyList<T>, IList
    {
        private sealed class DefaultIndexProvider : IIndexProvider
        {
            private readonly List<int> _freeIndexes;

            public int Current
            {
                get
                {
                    _freeIndexes.Sort();
                    if (_freeIndexes.Count == 0)
                        return -1;

                    return _freeIndexes[0];
                }
            }

            object IEnumerator.Current => Current;

            public DefaultIndexProvider(int size)
            {
                _freeIndexes = new List<int>(size);
                Reset();
            }

            public bool MoveNext()
            {
                if (_freeIndexes.Count == 0)
                    return false;

                _freeIndexes.RemoveAt(0);
                return true;
            }

            public void Reset()
            {
                _freeIndexes.Clear();
                for (int i = 0; i < _freeIndexes.Capacity; i++)
                {
                    _freeIndexes.Add(i);
                }
            }

            public void Dispose()
            {
            }

            public void Remove(int index)
            {
                _freeIndexes.Remove(index);
            }

            public void Add(int index)
            {
                _freeIndexes.Add(index);
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly SmartArray2D<T> _array;
            private int _index;

#pragma warning disable CS8766
            // ReSharper disable once ReturnTypeCanBeNotNullable
            public T? Current => _array[_index];
#pragma warning restore CS8766
            object? IEnumerator.Current => Current;

            public Enumerator(SmartArray2D<T> array)
            {
                _array = array;
                _index = -1;
            }

            public bool MoveNext()
            {
                if (_index + 1 >= _array.Length)
                    return false;

                _index++;
                return true;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose() { }
        }

        private readonly T[,] _array;
        private IIndexProvider _indexProvider;

        public T this[int x, int y]
        {
            get => _array[x, y];
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, default!))
                    _indexProvider.Add(x + y);
                else
                    _indexProvider.Remove(x + y);

                _array[x, y] = value;
            }
        }

        public T this[int index]
        {
            get
            {
                GetPosition(index, out int x, out int y);
                return this[x, y];
            }
            set
            {
                GetPosition(index, out int x, out int y);
                this[x, y] = value;
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value!;
        }

        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public bool IsReadOnly => false;
        public bool IsFixedSize => true;
        int ICollection<T>.Count => Length;
        int ICollection.Count => Length;
        int IReadOnlyCollection<T>.Count => Length;
        T IReadOnlyList<T>.this[int index] => this[index];
        public int Length => _array.GetLength(0)
                             * _array.GetLength(1);

        public SmartArray2D(int width, int height)
        {
            _array = new T[width, height];
            _indexProvider = new DefaultIndexProvider(Length);
            _indexProvider.Reset();
        }

        public void Clear()
        {
            (_array as IList).Clear();
            _indexProvider.Reset();
        }

        public bool Contains(T? item)
        {
            return (_array as IList).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            (_array as IList).CopyTo(array, arrayIndex);
        }

        public bool Remove(T? item)
        {
            int index = Array.BinarySearch(_array, item);
            if (index == -1)
                return false;

            RemoveAt(index);

            return true;
        }

        public int IndexOf(T? item)
        {
            return Array.BinarySearch(_array, item);
        }

        public void RemoveAt(int index)
        {
            this[index] = default!;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Insert(T item)
        {
            int index = _indexProvider.Current;
            if (index == -1)
                throw new IndexOutOfRangeException("todo"); //TODO Exception
            GetPosition(index, out int x, out int y);
            _array[x, y] = item;
            _indexProvider.MoveNext();
        }

        void IList<T>.Insert(int index, T item)
        {
            this[index] = item;
        }

        int IList.Add(object? value)
        {
            throw new NotSupportedException("Use indexer instead.");
        }

        bool IList.Contains(object? value)
        {
            return Contains((T)value!);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            (this as IList<T>).Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void ICollection<T>.Add(T? item)
        {
            throw new NotSupportedException("Use indexer instead.");
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void GetPosition(int index, out int x, out int y)
        {
            double rawValue = (double)index / _array.GetLength(0);
            y = (int)Math.Floor(rawValue);
            x = index - _array.GetLength(0) * y;
        }
    }
}