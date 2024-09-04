using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class VisualElementExtensions
    {
        public static void BindChoicesFromProperty(this PopupField<string> element, SerializedObject obj, string property)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo? listField = obj.targetObject.GetType().GetField(property, flags);
            if (listField == null)
            {
                Debug.LogError("TODO!!!"); //TODO
                return;
            }

            object objectList = listField.GetValue(obj.targetObject);
            if (objectList is not List<string> choices)
            {
                Debug.LogError("Todo2!!!"); //TODO
                return;
            }

            element.choices = choices;
        }

        public static void BindProperty(this BindableElement bindableElement, SerializedObject so, string bindingPath)
        {
            bindableElement.bindingPath = bindingPath;
            bindableElement.Bind(so);
        }

        public static void BindProperty(this PropertyField bindableElement, SerializedObject so, string bindingPath)
        {
            bindableElement.bindingPath = bindingPath;
            bindableElement.Bind(so);
        }

        public static void BindProperty(this BindableElement bindableElement, SerializedProperty property,
            string bindingPath)
        {
            bindableElement.bindingPath = bindingPath;
            bindableElement.BindProperty(property);
        }
    }
}