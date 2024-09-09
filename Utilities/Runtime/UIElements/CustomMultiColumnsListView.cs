using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public sealed class CustomMultiColumnsListView : MultiColumnListView
    {
        public new class UxmlFactory : UxmlFactory<CustomMultiColumnsListView, UxmlTraits> { }

        public void AddColumn(Column column)
        {
            columns.Add(column);
        }
    }
}