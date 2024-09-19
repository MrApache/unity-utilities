using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public sealed class MultiColumnsList : MultiColumnListView
    {
        public new class UxmlFactory : UxmlFactory<MultiColumnsList, UxmlTraits> { }

        public void AddColumn(Column column)
        {
            columns.Add(column);
        }
    }
}