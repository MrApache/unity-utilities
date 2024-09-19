using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Irisu.Utilities.UIElements
{
    public sealed class ToolbarLabel : Toolbar
    {
        private readonly Label _label;
        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        public ToolbarLabel(string label)
        {
            style.borderLeftWidth = 1;
            style.borderRightWidth = 1;
            style.borderBottomWidth = 1;
            _label = new Label
            {
                text = label,
                style =
                {
                    flexGrow = 1,
                    unityTextAlign = TextAnchor.MiddleCenter
                }
            };
            Add(_label);
        }
    }
}