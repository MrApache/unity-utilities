using UnityEngine;

namespace Irisu.Utilities
{
    public sealed class EditorOnlyCamera : MonoBehaviour
    {
        private void Awake()
        {
            if(Application.isPlaying)
                Destroy(gameObject);
        }
    }
}