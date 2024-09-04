using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    [FilePath("Library/VersionCounter.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class CounterData : ScriptableSingleton<CounterData>
    {
        [SerializeField, ReadOnly] private ulong _compilations;
        [SerializeField, ReadOnly] private ulong _builds;
        [SerializeField] private bool _disableDebugMessages;

        public ulong Compilations
        {
            get => _compilations;
            internal set
            {
                _compilations = value;
                Save();
            }
        }

        public ulong Builds
        {
            get => _builds;
            internal set
            {
                _builds = value;
                Save();
            }
        }

        public bool DisableDebugMessages
        {
            get => _disableDebugMessages;
            set
            {
                _disableDebugMessages = value;
                Save();
            }
        }

        public void Save()
        {
            Save(false);
        }
    }
}