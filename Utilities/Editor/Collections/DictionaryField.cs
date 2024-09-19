using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using Irisu.Utilities;
using Object = UnityEngine.Object;

/*
 * Add support:
 * Non unity objects
 * Vector2
 * Vector3
 * Vector4
 * Vector2Int
 * Vector3Int
 * Rect,
 * RectInt,
 * Bounds,
 */

#nullable enable

namespace Irisu.Collections
{
    public sealed class DictionaryField : MultiColumnListView
    {
        //Key type, Value type, Class type
        private static readonly Dictionary<Type, Dictionary<Type, Type>> _types;

        static DictionaryField()
        {
            _types = new Dictionary<Type, Dictionary<Type, Type>>
            {
                {
                    typeof(Object), new Dictionary<Type, Type>
                    {
                        { typeof(int), typeof(ObjectInt) },
                        { typeof(uint), typeof(ObjectUInt) },
                        { typeof(long), typeof(ObjectLong) },
                        { typeof(ulong), typeof(ObjectULong) },
                        { typeof(short), typeof(ObjectShort) },
                        { typeof(ushort), typeof(ObjectUShort) },
                        { typeof(byte), typeof(ObjectByte) },
                        { typeof(sbyte), typeof(ObjectSByte) },
                        { typeof(float), typeof(ObjectFloat) },
                        { typeof(double), typeof(ObjectDouble) },
                        { typeof(char), typeof(ObjectChar) },
                        { typeof(bool), typeof(ObjectBool) },
                        { typeof(string), typeof(ObjectString) },
                        { typeof(object), typeof(ObjectSystemObject) },
                        { typeof(Object), typeof(ObjectObject) }
                    }
                },
                /*
                {
                    typeof(object), new Dictionary<Type, Type>
                    {
                        { typeof(int),  typeof(SystemObjectInt) },
                        { typeof(uint), typeof(SystemObjectUInt) },
                        { typeof(long), typeof(SystemObjectLong) },
                        { typeof(ulong), typeof(SystemObjectULong) },
                        { typeof(short), typeof(SystemObjectShort) },
                        { typeof(ushort), typeof(SystemObjectUShort) },
                        { typeof(byte), typeof(SystemObjectByte) },
                        { typeof(sbyte), typeof(SystemObjectSByte) },
                        { typeof(float), typeof(SystemObjectFloat) },
                        { typeof(double), typeof(SystemObjectDouble) },
                        { typeof(char), typeof(SystemObjectChar) },
                        { typeof(bool), typeof(SystemObjectBool) },
                        { typeof(string), typeof(SystemObjectString) },
                        { typeof(Object), typeof(SystemObjectObject) },
                        { typeof(object), typeof(SystemObjectSystemObject) }
                    }
                }
            */
            };
        }

        private readonly Clickable _addItemCallback;
        private readonly Clickable _removeItemCallback;
        private readonly StyleSheet _fieldStyle;

        //Binding
        private IDictionary? _dictionary;
        private SerializedProperty? _property;

        private Type _keyType;
        private Type _valueType;
        private bool _isKeySystemType;
        private bool _isValueSystemType;

        private readonly List<Binding> _bindings;
        private IList _lazyItems; //Require for compability
        private IList _items; //Total items
        private IList _source; //Source items

        public DictionaryField(string title) : base(new Columns
        {
            new Column
            {
                name = "key",
                title = "Key",
                width = 95f,
                maxWidth = 95f
            },
            new Column
            {
                name = "value",
                title = "Value",
                stretchable = true,
            }
        })
        {
            _lazyItems = null!;
            _items = null!;
            _source = null!;

            _keyType = null!;
            _valueType = null!;

            _fieldStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Utilities/Utilities/Editor/Collections/field.uss");

            _bindings = new List<Binding>();

            headerTitle = title;
            showBorder = true;
            showFoldoutHeader = true;
            showAddRemoveFooter = true;
            showBoundCollectionSize = false;
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            selectionType = SelectionType.Multiple;

            this.Q<ScrollView>().mode = ScrollViewMode.Vertical;
            columns.resizable = false;
            columns.reorderable = false;

            /*
            AddItemCallback = new Clickable(Rebuild);
            RemoveItemCallback = new Clickable(Rebuild);
            */
            _addItemCallback = new Clickable(OnAddButtonClicked);
            _removeItemCallback = new Clickable(OnRemoveButtonClicked);

            RegisterCallback<GeometryChangedEvent>(FindFooterButtons);
            Rebuild();
        }

        private void FindFooterButtons(GeometryChangedEvent geometryChangedEvent)
        {
            if (showAddRemoveFooter)
            {
                this.Q<Button>("unity-list-view__add-button").clickable = _addItemCallback;
                this.Q<Button>("unity-list-view__remove-button").clickable = _removeItemCallback;
            }
        }

        private static bool IsValid(SerializedProperty property)
        {
            Type propertyType = property.GetExposedType();
            return propertyType.IsGenericType
                   && propertyType.GetGenericTypeDefinition() == typeof(SerializableDictionary<,>);
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

        public void Bind(SerializedProperty property)
        {
            if(!IsValid(property))
                throw new Exception("TODO");
            _property = property;
            _dictionary = GetDictionary(property);
            Type[] genericTypes = _dictionary!.GetType().GetGenericArguments();
            _keyType = genericTypes[0];
            _valueType = genericTypes[1];

            _isKeySystemType = _keyType.IsUnitySupportedType();
            _isValueSystemType = _valueType.IsUnitySupportedType();

            Type pairType = typeof(SerializablePair<,>).MakeGenericType(genericTypes);
            Type listType = typeof(List<>).MakeGenericType(pairType);
            _lazyItems = (IList)Activator.CreateInstance(listType);
            _items = (IList)Activator.CreateInstance(listType);
            _source = (IList)_dictionary.GetType()
                .GetPrivateField("_serializedPairs")!
                .GetValue(_dictionary);
            itemsSource = _items;

            foreach (object obj in _source)
                _items.Add(obj);

            for (int i = 0; i < _source.Count; i++)
                _bindings.Add(new Binding());

            Column key = columns["key"];
            key.makeCell = MakeKeyItem;
            key.bindCell = BindKeyItem;

            Column value = columns["value"];
            value.makeCell = MakeValueItem;
            value.bindCell = BindValueItem;

            Rebuild();
        }

        private bool IsLazyItem(int index) => index >= _source.Count;

        private bool ContainsItem(object target)
        {
            int copies = 0;
            foreach ((SerializedProperty key, SerializedProperty _ /* TODO!!! */) in _bindings)
            {
                object? propertyValue = key.GetPropertyValue();
                if (propertyValue == null)
                    continue;
                if (target == propertyValue)
                    copies++;
            }

            return copies > 1;
        }

        private SerializedObject CreateSerializedLazyItem()
        {
            Type keyType = typeof(Object).IsAssignableFrom(_keyType) ? typeof(Object) : typeof(object);
            Type valueType = typeof(Object).IsAssignableFrom(_valueType) ? typeof(Object) : _valueType;
            Type pairType = _types[keyType][valueType];
            ScriptableObject pairInstance = ScriptableObject.CreateInstance(pairType);
            return new SerializedObject(pairInstance);
        }

        private void BindKeyItem(VisualElement root, int index)
        {
            SerializedProperty property;
            IBindable field = (IBindable)root;

            if (IsLazyItem(index))
            {
                ObjectField objectField = (ObjectField)root;
                property = CreateSerializedLazyItem().FindProperty("Key");

                objectField.AddToClassList("dictionary-lazy-key-field-fail");
                objectField.AddToClassList("dictionary-lazy-key-field-success");
                objectField.AddToClassList("dictionary-lazy-key-field-contains");

                objectField.RegisterValueChangedCallback(evt =>
                {
                    object? value = evt.newValue;

                    if (value == null)
                    {
                        objectField.EnableInClassList("dictionary-lazy-key-field-fail", true);
                        objectField.EnableInClassList("dictionary-lazy-key-field-success", false);
                        objectField.EnableInClassList("dictionary-lazy-key-field-contains", false);
                        return;
                    }

                    if (ContainsItem(value))
                    {
                        objectField.EnableInClassList("dictionary-lazy-key-field-fail", false);
                        objectField.EnableInClassList("dictionary-lazy-key-field-success", false);
                        objectField.EnableInClassList("dictionary-lazy-key-field-contains", true);
                        return;
                    }

                    _lazyItems.RemoveAt(0);
                    _bindings.RemoveAt(index);
                    InsertItemInSourceList(value);
                });
            }
            else
            {
                string path = $"{_property!.name}._serializedPairs.Array.data[{index}]._key";
                property = _property.serializedObject.FindProperty(path);
            }

            field.BindProperty(property);
            Binding binding = _bindings[index];
            binding.Key = property;
        }

        private void BindValueItem(VisualElement root, int index)
        {
            Binding binding = _bindings[index];
            SerializedProperty property;
            IBindable field = (IBindable)root;
            if (IsLazyItem(index))
            {
                property = binding.Key.serializedObject.FindProperty("Value");
            }
            else
            {
                string path = $"{_property!.name}._serializedPairs.Array.data[{index}]._value";
                property = _property.serializedObject.FindProperty(path);
            }

            field.BindProperty(property);
            binding.Value = property;
        }

        private VisualElement MakeKeyItem()
        {
            VisualElement field;
            if (_isKeySystemType)
            {
                field = new PropertyField
                {
                    name = "key-field",
                    label = string.Empty
                };
            }
            else
            {
                field = new ObjectField
                {
                    name = "key-field",
                    label = string.Empty,
                    objectType = _keyType
                };
            }
            field.styleSheets.Add(_fieldStyle);
            field.AddToClassList("dictionary-field");
            field.AddToClassList("dictionary-key-field");
            return field;
        }

        private VisualElement MakeValueItem()
        {
            VisualElement field;
            if (_isValueSystemType)
            {
                field = new PropertyField
                {
                    name = "value-field",
                    label = string.Empty
                };
            }
            else
            {
                field = new ObjectField
                {
                    name = "value-field",
                    label = string.Empty,
                    objectType = _valueType
                };
            }
            field.styleSheets.Add(_fieldStyle);
            field.AddToClassList("dictionary-field");
            field.AddToClassList("dictionary-value-field");
            return field;
        }

        private void OnAddButtonClicked()
        {
            if (_dictionary == null)
                return;

            if(_keyType == typeof(int))
            {
                int value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if(_keyType == typeof(uint))
            {
                uint value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if(_keyType == typeof(long))
            {
                long value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if(_keyType == typeof(ulong))
            {
                ulong value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if(_keyType == typeof(short))
            {
                short value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if(_keyType == typeof(ushort))
            {
                ushort value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(float))
            {
                float value = 0;
                while (_dictionary.Contains(value))
                    value += 0.0001f;
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(byte))
            {
                byte value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(sbyte))
            {
                sbyte value = 0;
                while (_dictionary.Contains(value))
                    value++;
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(double))
            {
                double value = 0;
                while (_dictionary.Contains(value))
                    value += 0.001f;
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(string))
            {
                const string newEntry = "New_Entry_";
                int i = 0;
                string value = newEntry + i;
                while (_dictionary.Contains(value))
                    value = newEntry + ++i;
                InsertItemInSourceList(value);
            }
            /*
            else if (_keyType == typeof(Vector2))
            {
                Vector2 value = Vector2.zero;
                while(_dictionary.Contains(value))
                    value += new Vector2(1, 0);
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(Vector3))
            {
                Vector3 value = Vector3.zero;
                while(_dictionary.Contains(value))
                    value += new Vector3(1, 0, 0);
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(Vector4))
            {
                Vector4 value = Vector4.zero;
                while(_dictionary.Contains(value))
                    value += new Vector4(1, 0, 0, 0);
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(Vector2Int))
            {
                Vector2Int value = Vector2Int.zero;
                while(_dictionary.Contains(value))
                    value += new Vector2Int(1, 0);
                InsertItemInSourceList(value);
            }
            else if (_keyType == typeof(Vector3Int))
            {
                Vector3Int value = Vector3Int.zero;
                while(_dictionary.Contains(value))
                    value += new Vector3Int(1, 0, 0);
                InsertItemInSourceList(value);
            }
            */
            else if (typeof(Enum).IsAssignableFrom(_keyType))
            {
                Array values = Enum.GetValues(_keyType);
                if(_dictionary.Count == values.Length)
                    return;

                int value = 0;
                object enumValue = Enum.ToObject(_keyType, value);
                while (_dictionary.Contains(enumValue))
                    enumValue = Enum.ToObject(_keyType, ++value);

                InsertItemInSourceList(value);
            }
            else if (typeof(Object).IsAssignableFrom(_keyType))
            {
                AddLazyItem();
            }
            /*
            else if (typeof(object).IsAssignableFrom(_keyType))
            {
                AddLazyItem();
            }
            */
            else
            {
                throw new NotSupportedException();
            }
        }

        private void InsertItemInSourceList(object key)
        {
            Type constructedType = typeof(SerializablePair<,>).MakeGenericType(_keyType, _valueType);
            SerializedProperty listProperty = _property!.FindPropertyRelative("_serializedPairs");
            int index = listProperty.arraySize;
            listProperty.InsertArrayElementAtIndex(index);

            FieldInfo keyField = constructedType.FindField("_key")!;
            object pairInstance = Activator.CreateInstance(constructedType);
            keyField.SetValue(pairInstance, key);

            listProperty.GetArrayElementAtIndex(index).boxedValue = pairInstance;
            listProperty.serializedObject.ApplyModifiedProperties();

            _bindings.Add(new Binding());
            RefreshAllItems();
            Rebuild();
        }

        private void AddLazyItem()
        {
            Type constructedType = typeof(SerializablePair<,>).MakeGenericType(_keyType, _valueType);
            object pairInstance = Activator.CreateInstance(constructedType);
            _lazyItems.Add(pairInstance);
            _bindings.Add(new Binding());
            RefreshAllItems();
            Rebuild();
        }

        private void RefreshAllItems()
        {
            _items.Clear();
            foreach (object obj in _source)
                _items.Add(obj);
            foreach (object obj in _lazyItems)
                _items.Add(obj);
        }

        private void OnRemoveButtonClicked()
        {
            SerializedProperty listProperty = _property!.FindPropertyRelative("_serializedPairs");
            foreach (int index in selectedIndices.OrderByDescending(num => num))
            {
                _items.RemoveAt(index);
                if (IsLazyItem(index))
                {
                    _bindings.RemoveAt(index);
                    _lazyItems.RemoveAt(0);
                }
                else
                {
                    listProperty.DeleteArrayElementAtIndex(index);
                }
            }
            listProperty.serializedObject.ApplyModifiedProperties();
            Rebuild();
        }

        //Todo
        private sealed class Binding : IEquatable<SerializedProperty>
        {
            public SerializedProperty Key;
            public SerializedProperty Value;

            public Binding()
            {
                Key = null!;
                Value = null!;
            }

            public bool Equals(SerializedProperty other) => other == Key;

            public void Deconstruct(out SerializedProperty key, out SerializedProperty value)
            {
                key = Key;
                value = Value;
            }
        }

        private abstract class UnityLazyItem<TValue> : ScriptableObject
        {
            [SerializeField, UsedImplicitly] public Object Key;
            [SerializeField] public TValue? Value;

            protected UnityLazyItem() { Key = null!; }
        }
        private sealed class ObjectInt : UnityLazyItem<int> { }
        private sealed class ObjectUInt : UnityLazyItem<uint> { }
        private sealed class ObjectLong : UnityLazyItem<long> { }
        private sealed class ObjectULong : UnityLazyItem<ulong> { }
        private sealed class ObjectShort : UnityLazyItem<short> { }
        private sealed class ObjectUShort : UnityLazyItem<ushort> { }
        private sealed class ObjectByte : UnityLazyItem<byte> { }
        private sealed class ObjectSByte : UnityLazyItem<sbyte> { }
        private sealed class ObjectFloat : UnityLazyItem<float> { }
        private sealed class ObjectDouble : UnityLazyItem<double> { }
        private sealed class ObjectChar : UnityLazyItem<char> { }
        private sealed class ObjectBool : UnityLazyItem<bool> { }
        private sealed class ObjectString : UnityLazyItem<string> { }
        private sealed class ObjectSystemObject : UnityLazyItem<object> { }
        private sealed class ObjectObject : UnityLazyItem<Object> { }

        /*
        private abstract class SystemLazyItem<TValue> : ScriptableObject
        {
            [SerializeReference] public object Key;
            [SerializeField] public TValue? Value;
        }
        private sealed class SystemObjectInt : SystemLazyItem<int> { }
        private sealed class SystemObjectUInt : SystemLazyItem<uint> { }
        private sealed class SystemObjectLong : SystemLazyItem<long> { }
        private sealed class SystemObjectULong : SystemLazyItem<ulong> { }
        private sealed class SystemObjectShort : SystemLazyItem<short> { }
        private sealed class SystemObjectUShort : SystemLazyItem<ushort> { }
        private sealed class SystemObjectByte : SystemLazyItem<byte> { }
        private sealed class SystemObjectSByte : SystemLazyItem<sbyte> { }
        private sealed class SystemObjectFloat : SystemLazyItem<float> { }
        private sealed class SystemObjectDouble : SystemLazyItem<double> { }
        private sealed class SystemObjectChar : SystemLazyItem<char> { }
        private sealed class SystemObjectBool : SystemLazyItem<bool> { }
        private sealed class SystemObjectString : SystemLazyItem<string> { }
        private sealed class SystemObjectObject : SystemLazyItem<Object> { }
        private sealed class SystemObjectSystemObject : SystemLazyItem<object> { }
    */
    }
}