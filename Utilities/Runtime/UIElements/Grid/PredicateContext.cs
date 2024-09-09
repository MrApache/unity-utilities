using UnityEngine;

namespace Irisu.Utilities.UIElements
{
    public readonly struct PredicateContext
    {
        public readonly Vector2Int CellPosition;

        public PredicateContext(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }
    }
}