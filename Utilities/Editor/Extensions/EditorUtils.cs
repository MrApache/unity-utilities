using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        //With unity support
        public static bool IsUnitySupportedType(this Type type)
        {
            if (type == typeof(int))
                return true;
            if (type == typeof(uint))
                return true;
            if (type == typeof(long))
                return true;
            if (type == typeof(ulong))
                return true;
            if (type == typeof(short))
                return true;
            if (type == typeof(ushort))
                return true;
            if (type == typeof(byte))
                return true;
            if (type == typeof(sbyte))
                return true;
            if (type == typeof(float))
                return true;
            if (type == typeof(double))
                return true;
            if (type == typeof(char))
                return true;
            if (type == typeof(bool))
                return true;
            if (type == typeof(string))
                return true;
            if (type == typeof(Vector2))
                return true;
            if (type == typeof(Vector3))
                return true;
            if (type == typeof(Vector4))
                return true;
            if (type == typeof(Vector2Int))
                return true;
            if (type == typeof(Vector3Int))
                return true;
            if (typeof(Enum).IsAssignableFrom(type))
                return true;

            return false;
        }

        public static object? GetPropertyValue(this SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Boolean => property.boolValue,
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Color => property.colorValue,
                SerializedPropertyType.ObjectReference => property.objectReferenceValue,
                SerializedPropertyType.Enum => property.enumValueIndex,
                SerializedPropertyType.Vector2 => property.vector2Value,
                SerializedPropertyType.Vector3 => property.vector3Value,
                SerializedPropertyType.Vector4 => property.vector4Value,
                SerializedPropertyType.Rect => property.rectValue,
                SerializedPropertyType.ArraySize => property.arraySize,
                SerializedPropertyType.AnimationCurve => property.animationCurveValue,
                SerializedPropertyType.Bounds => property.boundsIntValue,
                SerializedPropertyType.Gradient => property.gradientValue,
                SerializedPropertyType.Quaternion => property.quaternionValue,
                SerializedPropertyType.ExposedReference => property.exposedReferenceValue,
                SerializedPropertyType.FixedBufferSize => property.fixedBufferSize,
                SerializedPropertyType.Vector2Int => property.vector2IntValue,
                SerializedPropertyType.Vector3Int => property.vector3Value,
                SerializedPropertyType.RectInt => property.rectValue,
                SerializedPropertyType.BoundsInt => property.boundsIntValue,
                SerializedPropertyType.ManagedReference => property.managedReferenceValue,
                SerializedPropertyType.Hash128 => property.hash128Value,
                /*
                SerializedPropertyType.Generic =>,
                SerializedPropertyType.LayerMask => ,
                SerializedPropertyType.Character => ,
                */
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static Type GetExposedType(this SerializedProperty property)
        {
            FieldInfo fi = GetField(property.serializedObject.targetObject, property.propertyPath)!; //TODO Fix??
            return fi.FieldType;
        }

        private static FieldInfo? GetField(object target, string fieldName)
        {
            Type? type = target.GetType();
            FieldInfo? field = null;
            while (type != null)
            {
                field = type.FindField(fieldName);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
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
