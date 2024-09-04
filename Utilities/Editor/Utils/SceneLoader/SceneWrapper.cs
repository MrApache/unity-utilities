using System;
using UnityEditor;
using UnityEngine;

namespace Irisu.Utilities
{
    [Serializable]
    public sealed class SceneWrapper
    {
        [SerializeField] private SceneAsset _scene;
        [SerializeField] private bool _unloadScene;

        public SceneAsset Scene => _scene;
        public bool UnloadScene => _unloadScene;

        public SceneWrapper()
        {
            _scene = null!;
        }
    }
}
