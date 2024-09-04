using UnityEngine.UIElements;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class VisualElementExtensions
    {
        public static void Enable(this VisualElement element, bool value)
        {
            element.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
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