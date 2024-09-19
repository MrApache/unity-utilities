using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public sealed class MultiColumnsList : MultiColumnListView
    {
        public new class UxmlFactory : UxmlFactory<MultiColumnsList, UxmlTraits> { }

        private readonly List<VisualElement> _headerClickableElements;

        private bool _stopColumnLeftClickInteraction;
        public bool StopColumnLeftClickInteraction
        {
            get => _stopColumnLeftClickInteraction;
            set
            {
                _stopColumnLeftClickInteraction = value;
                DisableLeftClickHeaderInteraction(value);
            }
        }

        private bool _stopColumnRightClickInteraction;
        private bool _subscribed;
        public bool StopColumnRightClickInteraction
        {
            get => _stopColumnRightClickInteraction;
            set
            {
                _stopColumnRightClickInteraction = value;
                switch (value)
                {
                    case true when !_subscribed:
                        headerContextMenuPopulateEvent += ClearContextMenuItems;
                        _subscribed = true;
                        break;
                    case false when _subscribed:
                        headerContextMenuPopulateEvent -= ClearContextMenuItems;
                        _subscribed = false;
                        break;
                }
            }
        }

        public MultiColumnsList()
        {
            _headerClickableElements = new List<VisualElement>();
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        public void AddColumn(Column column)
        {
            columns.Add(column);
        }

        private void OnGeometryChange(GeometryChangedEvent geometryChangedEvent)
        {
            _headerClickableElements.Clear();
            List<VisualElement> queryResult = this.Query(className: "unity-multi-column-header__column").ToList();
            _headerClickableElements.AddRange(queryResult);
            foreach (VisualElement element in queryResult)
                _headerClickableElements.AddRange(element.Children());

            DisableLeftClickHeaderInteraction(_stopColumnLeftClickInteraction);
        }

        private void DisableLeftClickHeaderInteraction(bool disable)
        {
            foreach (VisualElement clickableElement in _headerClickableElements)
                clickableElement.pickingMode = disable ? PickingMode.Ignore : PickingMode.Position;
        }

        private void ClearContextMenuItems(ContextualMenuPopulateEvent evt, Column column)
        {
            evt.menu.ClearItems();
        }
    }
}