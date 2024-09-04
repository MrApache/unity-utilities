using UnityEditor;
using UnityEngine.UIElements;

namespace Irisu.Collections
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public sealed class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializableDictionaryField field = new SerializableDictionaryField(property.displayName);
            field.Bind(property);
            return field;
        }
    }
}