using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Irisu.Utilities.UIElements
{
    public sealed class CellArray : IEnumerable<GridCell>
    {
        private GridCell[,] _array;

        public int LengthX => _array.GetLength(0);
        public int LengthY => _array.GetLength(1);
        public int Length => LengthX * LengthY;

        public GridCell this[int x, int y]
        {
            get => _array[x, y];
            set => _array[x, y] = value;
        }

        public GridCell this[int index]
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

        public CellArray()
        {
            _array = new GridCell[0, 0];
        }

        public ResizeResult Resize(int x, int y)
        {
            List<GridCell> addedCells = new List<GridCell>();
            List<GridCell> removedCells = new List<GridCell>();
            GridCell[,] newArray = new GridCell[x, y];
            Enumerator enumerator = GetEnumerator();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    GridCell cell;
                    if (!enumerator.MoveNext())
                    {
                        cell = new GridCell();
                        addedCells.Add(cell);
                    }
                    else
                    {
                        cell = enumerator.Current;
                    }
                    cell.Position = new Vector2Int(i, j);
                    newArray[i, j] = cell;
                }
            }
            while (enumerator.MoveNext())
                removedCells.Add(enumerator.Current);
            enumerator.Dispose();
            _array = newArray;
            return new ResizeResult(addedCells, removedCells);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public Enumerator GetEnumerator() => new Enumerator(this);
        IEnumerator<GridCell> IEnumerable<GridCell>.GetEnumerator() => new Enumerator(this);

        private void GetPosition(int index, out int x, out int y)
        {
            double rawValue = (double)index / LengthX;
            y = (int)Math.Floor(rawValue);
            x = index - LengthX * y;
        }

        public readonly struct ResizeResult
        {
            public readonly List<GridCell> Added;
            public readonly List<GridCell> Removed;

            public ResizeResult(List<GridCell> added, List<GridCell> removed)
            {
                Added = added;
                Removed = removed;
            }
        }

        public struct Enumerator : IEnumerator<GridCell>
        {
            private readonly CellArray _array;
            private int _indexX;
            private int _indexY;

            public GridCell Current => _array[_indexX, _indexY];
            object IEnumerator.Current => Current;

            public Enumerator(CellArray array)
            {
                _array = array;
                _indexX = 0;
                _indexY = -1;
            }

            public bool MoveNext() => MovePointer();

            private bool MovePointer()
            {
                if (_indexY + 1 >= _array.LengthY
                    && _indexX + 1 >= _array.LengthX)
                    return false;

                if (_indexY + 1 == _array.LengthY)
                {
                    MovePointer(_indexX + 1, 0);
                    return true;
                }

                MovePointer(_indexX, _indexY + 1);
                return true;
            }

            private void MovePointer(int x, int y)
            {
                _indexX = x;
                _indexY = y;
            }

            public void Reset()
            {
                _indexX = 0;
                _indexY = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}