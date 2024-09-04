using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Irisu.Utilities;
#nullable enable

namespace Irisu.Collections
{
    public class SerializableDictionaryField : BaseSerializableDictionaryField
    {
        private IDictionary? _dictionary { get; set; }
        private SerializedProperty? _property;

        public SerializableDictionaryField(string title) : base(title)
        {
            AddItemCallback = new Clickable(OnAddButtonClicked);
            RemoveItemCallback = new Clickable(OnRemoveButtonClicked);
        }

        private static bool IsValid(SerializedProperty property)
        {
            Type propertyType = property.GetExposedType();
            return propertyType.IsGenericType
                   && propertyType.GetGenericTypeDefinition() == typeof(SerializableDictionary<,>);
        }

        public void Bind(SerializedProperty property)
        {
            if(!IsValid(property)) throw new Exception("TODO");
            _property = property;
            _dictionary = GetDictionary(property);
            itemsSource = (IList)_dictionary.GetType()
                .GetPrivateField("_serializedPairs")!
                .GetValue(_dictionary);
            BindColumns();
            Rebuild();
        }

        private void BindColumns()
        {
            Column key = columns["key"];
            key.makeCell = MakeKeyItem;
            key.bindCell = BindKeyItem;

            Column value = columns["value"];
            value.makeCell = MakeValueItem;
            value.bindCell = BindValueItem;
        }

        private static IDictionary GetDictionary(SerializedProperty property)
        {
            const string arrayIndex = "___index";
            object targetObject = property.serializedObject.targetObject;
            string[] fields = property.propertyPath.Replace("Array.data[", arrayIndex).Split('.');
            foreach (string fieldName in fields)
            {
                if (fieldName.Contains(arrayIndex))
                {
                    string stringIndexWithBracket = fieldName.Replace(arrayIndex, string.Empty);
                    ReadOnlySpan<char> stringIndex = stringIndexWithBracket.AsSpan(0, stringIndexWithBracket.Length - 1);
                    int index = int.Parse(stringIndex);
                    targetObject = ((IList)targetObject)[index];
                    continue;
                }

                FieldInfo? field = GetField(targetObject, fieldName);
                if (field == null)
                    throw new Exception($"Field {fieldName} not found in {targetObject.GetType()}");
                targetObject = field.GetValue(targetObject);
            }

            return (IDictionary)targetObject;
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

        private void BindKeyItem(VisualElement root, int index)
            => Bind(root, index, "key-field", "_key");

        private void BindValueItem(VisualElement root, int index)
            => Bind(root, index, "value-field", "_value");

        private void Bind(VisualElement root, int index, string name, string bindingPath)
        {
            string path = $"{_property!.name}._serializedPairs.Array.data[{index}]";
            PropertyField field = root.Q<PropertyField>(name);
            field.BindProperty(_property.serializedObject, $"{path}.{bindingPath}");
            //field.SetEnabled(_properties.AllowElementEdit);
            SetFieldStyle(field);
        }

        private void SetFieldStyle(PropertyField field)
        {
            VisualElement? fieldElement = field.Q(className: "unity-base-field__input");
            if (fieldElement == null)
                return;
            IStyle style = fieldElement.style;
            style.borderBottomLeftRadius = 0;
            style.borderBottomRightRadius = 0;
            style.borderTopRightRadius = 0;
            style.borderTopLeftRadius = 0;
        }

        private static VisualElement MakeKeyItem()
        {
            PropertyField keyField = new PropertyField
            {
                name = "key-field",
                label = string.Empty,
                style =
                {
                    paddingRight = 4f
                }
            };

            return keyField;
        }

        private static VisualElement MakeValueItem()
        {
            PropertyField valueField = new PropertyField
            {
                name = "value-field",
                label = string.Empty,
                style =
                {
                    flexGrow = 1,
                    paddingRight = 8f
                }
            };
            return valueField;
        }

        private void OnAddButtonClicked()
        {
            Type[] genericTypes = _dictionary!.GetType().GetGenericArguments();
            Type constructedType = typeof(SerializablePair<,>).MakeGenericType(genericTypes);
            SerializedProperty listProperty = _property!.FindPropertyRelative("_serializedPairs");
            int index = listProperty.arraySize;
            listProperty.InsertArrayElementAtIndex(index);
            object instance = Activator.CreateInstance(constructedType);
            if(genericTypes[0] == typeof(int))
            {
                int value = 0;
                while (_dictionary.Contains(value))
                    value++;
                instance.GetType().FindField("_key")!.SetValue(instance, value);
            }
            listProperty.GetArrayElementAtIndex(index).boxedValue = instance;
            listProperty.serializedObject.ApplyModifiedProperties();
            Rebuild();
        }

        private void OnRemoveButtonClicked()
        {
            SerializedProperty listProperty = _property!.FindPropertyRelative("_serializedPairs");
            foreach (int index in selectedIndices)
                listProperty.DeleteArrayElementAtIndex(index);
            listProperty.serializedObject.ApplyModifiedProperties();
            Rebuild();
        }
    }
}