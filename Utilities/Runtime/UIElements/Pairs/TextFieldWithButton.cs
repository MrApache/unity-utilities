using UnityEngine;
using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    internal sealed class TextFieldWithButton : ElementPair<TextField, Button>
    {
        public TextFieldWithButton() : this(string.Empty)
        { }

        public TextFieldWithButton(string buttonText)
        {
            Second.text = buttonText;
            Second.style.flexGrow = 1;
            First.style.flexGrow = 1;
            First.Q("unity-text-input").style.unityTextAlign = TextAnchor.MiddleCenter;
        }
    }
}