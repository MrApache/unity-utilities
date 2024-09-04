using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Irisu.Utilities
{
    [PublicAPI]
    public readonly struct LazyProperty<T> where T : Object
    {
        public readonly VisualElement Root;

        public LazyProperty(VisualElement propertyRoot)
        {
            Root = propertyRoot;
        }

        public void Bind(SerializedObject serializedObject)
        {
            if (serializedObject.targetObject is not T)
                throw new InvalidAssetType(serializedObject.targetObject, typeof(T));

            foreach (VisualElement element in Root.Children())
            {
                PropertyField field = (PropertyField)element;
                field.BindProperty(serializedObject, field.name);
            }
        }
    }
}