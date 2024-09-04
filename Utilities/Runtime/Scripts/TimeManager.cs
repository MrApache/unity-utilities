using UnityEngine;

namespace Irisu.Utilities.Scripts
{
    public sealed class TimeManager : MonoBehaviour
    {
        [Range(0f, 1f)] [SerializeField] private float _time = 1;

        private void Update()
        {
            Time.timeScale = _time;
        }
    }
}