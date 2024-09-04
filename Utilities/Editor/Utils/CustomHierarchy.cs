using UnityEngine;
using UnityEditor;

namespace Irisu.Utilities
{
    [InitializeOnLoad]
    public sealed class CustomHierarchy : MonoBehaviour
    {
        private const string ImportantObjectPattern = "###";
        private static readonly Color _importantObjectColor = new Color32(28, 25, 25, 255);

        static CustomHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
            if (obj == null)
                return;

            if (!obj.name.StartsWith(ImportantObjectPattern))
                return;

            EditorGUI.DrawRect(selectionRect, _importantObjectColor);
            string nameWithoutPattern = obj.name[3..];
            DrawEntry(selectionRect, nameWithoutPattern);
        }

        private static void DrawEntry(Rect selectionRect, string name)
        {
            int fontSize = GUI.skin.font.fontSize;
            float nameWidth = GetNameWidth(name);
            float offsetX = selectionRect.x + (selectionRect.width / 2 - nameWidth / 2);
            Rect labelRect = new Rect(selectionRect)
            {
                x = offsetX,
            };

            GUIStyle guiStyle = new GUIStyle
            {
                normal = new GUIStyleState { textColor = Color.white },
                fontStyle = FontStyle.Bold
            };

            EditorGUI.LabelField(selectionRect, "【", guiStyle);
            EditorGUI.LabelField(labelRect, name, guiStyle);

            Rect bracketRect = new Rect(selectionRect)
            {
                x = selectionRect.xMax - fontSize,
            };

            EditorGUI.LabelField(bracketRect, "】", guiStyle);
        }

        private static float GetNameWidth(string name)
        {
            float sum = 0;
            foreach (char ch in name)
            {
                GUI.skin.font.GetCharacterInfo(ch, out CharacterInfo info);
                sum += info.advance;
            }
            return sum;
        }
    }
}