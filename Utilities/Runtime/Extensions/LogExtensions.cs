using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class LogExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(this IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Debug.Log(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning<T>(this IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Debug.LogWarning(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError<T>(this IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                Debug.LogError(item);
            }
        }
    }
}
