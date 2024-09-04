using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Irisu.Utilities
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public sealed class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            PropertyField field = new PropertyField(property);
            field.Bind(property.serializedObject);
            field.SetEnabled(false);
            return field;
        }
    }
}
