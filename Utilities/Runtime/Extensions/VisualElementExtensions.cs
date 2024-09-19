using UnityEngine.UIElements;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class VisualElementExtensions
    {
        public static T AddTo<T>(this T element, VisualElement root) where T : VisualElement
        {
            root.Add(element);
            return element;
        }

        public static T Enable<T>(this T element, bool enable) where T : VisualElement
        {
            element.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            return element;
        }

        public static bool IsEnabled(this VisualElement element)
        {
            return element.style.display == DisplayStyle.Flex;
        }

        public static void RemoveElement(this VisualElement root, string name)
        {
            VisualElement element = root.Q(name);
            root.Remove(element);
        }

        public static TemplateContainer InstantiateWithName(this VisualTreeAsset asset, string elementName)
        {
            TemplateContainer clone = asset.Instantiate();
            clone.name = elementName;
            return clone;
        }
    }
}