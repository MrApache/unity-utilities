using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Irisu.Utilities
{
    [FilePath("Library/SceneLoader.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class SceneLoaderData : ScriptableSingleton<SceneLoaderData>, IList, IList<SceneWrapper>
    {
        [SerializeField] private List<SceneWrapper> _scenes;

        object IList.this[int index]
        {
            get => _scenes[index];
            set
            {
                if (value is not SceneWrapper sceneWrapper)
                    throw new InvalidOperationException();

                this[index] = sceneWrapper;
            }
        }

        public SceneWrapper this[int index]
        {
            get => _scenes[index];
            set
            {
                _scenes[index] = value;
                Save();
            }
        }

        public bool IsFixedSize => false;
        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public int Count => _scenes.Count;
        public object SyncRoot => this;

        public SceneLoaderData()
        {
            _scenes = new List<SceneWrapper>();
        }

        IEnumerator<SceneWrapper> IEnumerable<SceneWrapper>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<SceneWrapper>.Enumerator GetEnumerator()
        {
            return _scenes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _scenes.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public int Add(object? value)
        {
            Debug.Log($"try add: {value?.GetType().Name}");
            if (value is not SceneWrapper sceneWrapper)
                return -1;

            Add(sceneWrapper);
            return _scenes.Count - 1;
        }

        public void Add(SceneWrapper item)
        {
            _scenes.Add(item);
            Save();
        }

        public void Clear()
        {
            _scenes.Clear();
            Save();
        }

        public bool Contains(SceneWrapper item)
        {
            return _scenes.Contains(item);
        }

        public void CopyTo(SceneWrapper[] array, int arrayIndex)
        {
            _scenes.CopyTo(array, arrayIndex);
        }

        public bool Contains(object? value)
        {
            return value is SceneWrapper sceneWrapper
                   && _scenes.Contains(sceneWrapper);
        }

        public int IndexOf(object value)
        {
            if (value is not SceneWrapper sceneWrapper)
                return -1;

            return _scenes.IndexOf(sceneWrapper);
        }

        public void Insert(int index, object value)
        {
            if (value is not SceneWrapper sceneWrapper)
                throw new ArgumentException();

            Insert(index, sceneWrapper);
        }

        public void Remove(object value)
        {
            if (value is not SceneWrapper sceneWrapper)
                throw new ArgumentException();

            Remove(sceneWrapper);
        }

        public int IndexOf(SceneWrapper item)
        {
            return _scenes.IndexOf(item);
        }

        public void Insert(int index, SceneWrapper item)
        {
            _scenes.Insert(index, item);
            Save();
        }

        public void RemoveAt(int index)
        {
            _scenes.RemoveAt(index);
            Save();
        }

        public bool Remove(SceneWrapper item)
        {
            bool result = _scenes.Remove(item);
            Save();
            return result;
        }

        public static void Save()
        {
            instance.Save(false);
            instance.SetFirstScene();
        }

        private void SetFirstScene()
        {
            if(_scenes.Count == 0)
                return;

            EditorSceneManager.playModeStartScene = _scenes[0].Scene;
        }
    }
}