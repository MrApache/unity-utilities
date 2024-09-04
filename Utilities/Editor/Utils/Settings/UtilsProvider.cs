using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Irisu.Utilities;

namespace Irisu.Utilities
{
    internal static class UtilsProvider
    {
        private const string Path = "Project/CoreUtils Manager";
        private const string AssetPath = "Assets/CoreUtils/Editor/Utils/Settings/utils-manager.uxml";

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider(Path, SettingsScope.Project)
            {
                label = "CoreUtils Manager",
                activateHandler = OnActivateSettings,
                keywords = new HashSet<string>(new[]
                    { "CoreUtils", "Counter", "Scene" })
            };

            return provider;
        }

        private static void OnActivateSettings(string arg1, VisualElement root)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetPath);
            asset.CloneTree(root);

            BindCounterSettings(root);
            BindSceneLoaderSettings(root);
        }

        private static void BindCounterSettings(VisualElement root)
        {
            SerializedObject so = new SerializedObject(CounterData.instance);

            Toggle dmesgOption = root.Q<Toggle>("disable-dmesg-option");
            dmesgOption.BindProperty(so, "_disableDebugMessages");

            root.Q<Button>("reset-counter-button").clicked += () =>
            {
                bool reset = EditorUtility.DisplayDialog("Reset counter?",
                    "Are you sure you want to reset the counter?"
                    , "Yes", "No");

                if (!reset)
                    return;

                CounterData.instance.Compilations = 0;
                CounterData.instance.Builds = 0;
            };
        }

        private static void BindSceneLoaderSettings(VisualElement root)
        {
            SerializedObject so = new SerializedObject(SceneLoaderData.instance);
            ListView listView = root.Q<ListView>("scenes");
            listView.BindProperty(so, "_scenes");
            listView.makeItem = () =>
            {
                VisualElement container = new VisualElement
                    { style = { flexDirection = FlexDirection.Row } };
                ObjectField sceneField = new ObjectField
                    { name = "SceneField", objectType = typeof(SceneAsset) };
                Toggle boolField = new Toggle
                    { name = "BoolField" };

                container.Add(sceneField);
                container.Add(boolField);

                return container;
            };

            listView.bindItem = (element, i) =>
            {
                string path = $"_scenes.Array.data[{i}]";

                ObjectField? sceneField = (ObjectField)element.Q("SceneField");
                sceneField.BindProperty(so, $"{path}._scene");

                Toggle? boolField = (Toggle)element.Q("BoolField");
                boolField.BindProperty(so, $"{path}._unloadScene");

                SceneLoaderData.Save();
            };
        }
    }
}