using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using Object = UnityEngine.Object;
#nullable enable

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class EditorUtils
    {
        public static Type GetExposedType(this SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo fi = parentType.FindField(property.propertyPath)!;
            return fi.FieldType;
        }
        
        public static VisualElement QRemove(this VisualElement element, string elementName)
        {
            VisualElement result = element.Q(elementName);
            element.Remove(result);
            return result;
        }

        public static VisualElement DrawDefaultProperty(this SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            Type type = property.serializedObject.targetObject.GetType();
            FieldInfo[] fields = type
                .GetAllFields()
                .Where(f => f.IsPublic || f.ContainsAttribute<SerializeField>() && !f.IsStatic)
                .ToArray();

            foreach (FieldInfo field in fields)
            {
                PropertyField propertyField = CreatePropertyField(property.serializedObject,
                    $"{property.propertyPath}.{field.Name}", IsReadOnly(field));
                propertyField.name = field.Name;
                root.Add(propertyField);
            }

            return root;
        }

        public static VisualElement DrawDefaultSerializedObject(this SerializedObject so)
        {
            VisualElement root = new VisualElement();

            FieldInfo[] fields = so.targetObject.GetType()
                .GetAllFields()
                .Where(f => f.IsPublic || f.ContainsAttribute<SerializeField>() && !f.IsStatic)
                .ToArray();

            foreach (FieldInfo field in fields)
            {
                if (field.ContainsAttribute<HideInInspector>())
                    continue;
                PropertyField propertyField = CreatePropertyField(so, $"{field.Name}", IsReadOnly(field));
                propertyField.name = field.Name;
                root.Add(propertyField);
            }

            return root;
        }

        public static LazyProperty<T> DrawDefaultSerializedObject<T>() where T : Object
        {
            VisualElement root = new VisualElement();

            FieldInfo[] fields = typeof(T)
                .GetAllFields()
                .Where(f => f.IsPublic || f.ContainsAttribute<SerializeField>() && !f.IsStatic)
                .ToArray();

            foreach (FieldInfo field in fields)
            {
                if (field.ContainsAttribute<HideInInspector>())
                    continue;

                bool isReadOnly = IsReadOnly(field);
                PropertyField propertyField = new PropertyField();
                propertyField.SetEnabled(!isReadOnly);
                propertyField.name = field.Name;
                root.Add(propertyField);
            }

            return new LazyProperty<T>(root);
        }

        public static PropertyField CreatePropertyField(this SerializedObject so, string bindingPath, bool isReadOnly = false)
        {
            PropertyField field = new PropertyField();
            field.SetEnabled(!isReadOnly);
            field.bindingPath = bindingPath;
            field.Bind(so);
            return field;
        }

        public static PropertyField CreatePropertyField(this SerializedProperty prop, string? bindingPath, bool isReadonly = false)
        {
            string path = string.IsNullOrEmpty(bindingPath)
                ? prop.propertyPath
                : $"{prop.propertyPath}.{bindingPath}";

            PropertyField field = new PropertyField();
            field.SetEnabled(!isReadonly);
            field.bindingPath = path;
            field.Bind(prop.serializedObject);
            return field;
        }

        private static bool IsReadOnly(FieldInfo field) 
        {
            return field.GetCustomAttribute<ReadOnlyAttribute>() != null;
        }
    }
}
