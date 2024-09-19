using UnityEditor;
using UnityEngine.UIElements;

namespace Irisu.Collections
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public sealed class DictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            DictionaryField field = new DictionaryField(property.displayName);
            field.Bind(property);
            return field;
        }
    }
}