using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public class ElementPair<T1, T2> : VisualElement
        where T1 : VisualElement, new()
        where T2 : VisualElement, new()
    {
        public readonly T1 First;
        public readonly T2 Second;

        public ElementPair()
        {
            First = new T1();
            Second = new T2();
            First.Enable(false);
            Add(Second);
            Add(First);
        }
    }
}