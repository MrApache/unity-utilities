using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class ReflectionExtensions
    {
        public const BindingFlags PublicFlags = BindingFlags.Instance | BindingFlags.Public;
        public const BindingFlags PrivateFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        public const BindingFlags PublicStaticFlags = PublicFlags | BindingFlags.Static;
        public const BindingFlags PrivateStaticFlags = PrivateFlags | BindingFlags.Static;
        public const BindingFlags AllFlags = PublicFlags | PrivateFlags | BindingFlags.Static;

        public static bool ContainsAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttribute<T>() != null;
        }

        public static FieldInfo? GetPublicField(this Type type, string name)
        {
            return type.GetField(name, PublicStaticFlags);
        }

        public static FieldInfo? GetPrivateField(this Type type, string name)
        {
            return type.GetField(name, PrivateStaticFlags);
        }

        public static FieldInfo[] GetPublicFields(this Type type, bool includeStatic = false)
        {
            BindingFlags flags = includeStatic ? PublicStaticFlags : PublicFlags;
            return type.GetFields(flags);
        }

        public static FieldInfo[] GetPrivateFields(this Type type, bool includeStatic = false)
        {
            BindingFlags flags = includeStatic ? PrivateStaticFlags : PrivateFlags;
            return type.GetFields(flags);
        }

        public static FieldInfo[] GetAllFields(this Type type)
        {
            return type.GetFields(AllFlags);
        }

        [CanBeNull]
        public static FieldInfo FindField(this Type type, string name)
        {
            return type.GetField(name, AllFlags);
        }
    }
}