using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class BaseTypeExtensions
    {
        public static bool ToBool(this int value)
        {
            return value > 0;
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}