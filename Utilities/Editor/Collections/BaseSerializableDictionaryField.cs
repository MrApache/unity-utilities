using UnityEngine.UIElements;

namespace Irisu.Collections
{
    public abstract class BaseSerializableDictionaryField : MultiColumnListView
    {
        public Clickable AddItemCallback;
        public Clickable RemoveItemCallback;

        public new bool showAddRemoveFooter
        {
            get => base.showAddRemoveFooter;
            set
            {
                base.showAddRemoveFooter = value;
                if (value)
                {
                    this.Q<Button>("unity-list-view__add-button").clickable = AddItemCallback;
                    this.Q<Button>("unity-list-view__remove-button").clickable = RemoveItemCallback;
                }
            }
        }

        protected BaseSerializableDictionaryField(string title) : base(new Columns
        {
            new Column
            {
                name = "key",
                title = "Key",
                width = 80f,
                maxWidth = 80f,
            },
            new Column
            {
                name = "value",
                title = "Value",
                stretchable = true,
            }
        })
        {
            headerTitle = title;
            showBorder = true;
            showFoldoutHeader = true;
            showBoundCollectionSize = false;
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;

            this.Q<ScrollView>().mode = ScrollViewMode.Vertical;
            columns.resizable = false;
            columns.reorderable = false;

            showAddRemoveFooter = true;

            AddItemCallback = new Clickable(Rebuild);
            RemoveItemCallback = new Clickable(Rebuild);
            Rebuild();
        }
    }
}