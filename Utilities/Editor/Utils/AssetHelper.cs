using System;
using UnityEditor;
using UnityEngine;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Irisu.Utilities
{
    [PublicAPI]
    public static class AssetHelper
    {
        public static string GetAssetGUID(Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            return GUIDFromAssetPath(assetPath);
        }

        public static T LoadAsset<T>(string guid) where T : Object
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static string GUIDFromAssetPath(string assetPath)
        {
            return AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
        }

        public static string GetAssetName(string source)
        {
            const char separator = '/';
            const char endChar = '.';
            int indexOfLastSeparator = source.LastIndexOf(separator);
            int indexOfEndChar = source.LastIndexOf(endChar);
            int substringLength = indexOfEndChar - indexOfLastSeparator;
            return source.Substring(indexOfLastSeparator + 1, substringLength - 1);
        }

        public static string GetFolderPath(string source)
        {
            const char endChar = '/';
            int indexOfLastChar = source.LastIndexOf(endChar);
            return source[..indexOfLastChar];
        }

        public static void RenameAsset(Object obj, string newName)
        {
            string fullPath = AssetDatabase.GetAssetPath(obj);
            AssetDatabase.RenameAsset(fullPath, newName);
        }

        public static bool DeleteAsset(Object obj)
        {
            string? path = AssetDatabase.GetAssetPath(obj);
            return !string.IsNullOrEmpty(path)
                   && AssetDatabase.DeleteAsset(path);
        }

        public static bool DeleteAsset(GUID guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.DeleteAsset(assetPath);
        }

        public static bool DeleteAsset(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.DeleteAsset(assetPath);
        }

        public static T? FindAndGetAsset<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0)
                return default;
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public static string[] GetAllAssets<T>() where T : Object
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        }

        public static T[] LoadAllAssets<T>() where T : Object
        {
            T[] array = Array.Empty<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (guids.Length == 0)
                return array;

            array = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                array[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }

            return array;
        }

        public static T? CreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            return asset;
        }

        public static T? CreateAsset<T>(string defaultName, out string assetPath) where T : ScriptableObject
        {
            assetPath = EditorUtility.SaveFilePanelInProject("Select path", defaultName, "asset", string.Empty);
            return assetPath.Equals(string.Empty)
                ? null
                : CreateAsset<T>(assetPath);
        }

        public static void SaveAssetInProject<T>(T asset, string defaultName) where T : ScriptableObject
        {
            string path = EditorUtility.SaveFilePanelInProject("Select path", defaultName, "asset", string.Empty);
            SaveAssetInProjectWithoutConfirmation(asset, path);
        }

        public static void SaveAssetInProjectWithoutConfirmation<T>(T asset, string path) where T : ScriptableObject
        {
            if (path.Equals(string.Empty))
                return;

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.Refresh();
        }

        public static void SaveAsset(Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssetIfDirty(obj);
        }
    }
}