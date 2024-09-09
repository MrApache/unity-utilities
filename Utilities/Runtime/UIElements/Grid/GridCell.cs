using UnityEngine;
using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public class GridCell : GridElements
    {
        public Vector2Int Position { get; internal set; }

        public GridCell() : this(0, 0) {}

        public GridCell(int xPos, int yPos)
        {
            Position = new Vector2Int(xPos, yPos);
        }

        protected override void OnAdd(VisualElement element)
        {
            element.StretchToParentSize();
        }
    }
}