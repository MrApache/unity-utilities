using UnityEngine.UIElements;

namespace Irisu.Utilities.UIElements
{
    public interface IElementFactory
    {
        public VisualElement Instantiate();
        public void Bind(VisualElement element);
        public void Unbind(VisualElement element);
        public void Reset();
    }
}