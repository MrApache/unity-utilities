using System;
using UnityEngine;

namespace Irisu.Utilities
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ReadOnlyAttribute : PropertyAttribute
    {
    }
}
