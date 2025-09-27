#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Moonstone.HierarchyCustomization
{
    using Separator = HierarchyCustomizationData.Separator;

    [InitializeOnLoad]
    public static class HierarchyCustomization
    {
        const string SETTING_FILENAME = "Hierarchy Customization";

        static readonly HierarchyCustomizationData data;

        static HierarchyCustomization()
        {
            data = LoadSettings();

            EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        static HierarchyCustomizationData LoadSettings()
        {
            var dataPath = $"Assets/Settings/Data/{SETTING_FILENAME}.asset";
            var data = AssetDatabase.LoadAssetAtPath<HierarchyCustomizationData>(dataPath);
            if (data == null)
            {
                var directory = System.IO.Path.GetDirectoryName(dataPath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                    AssetDatabase.Refresh();
                }
                data = ScriptableObject.CreateInstance<HierarchyCustomizationData>();
                AssetDatabase.CreateAsset(data, dataPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return data;
        }

        [MenuItem("Tools/Moonstone/Hierarchy Customization/Edit Settings")]
        public static void EditSettingsMenuItem()
        {
            Selection.activeObject = data;
        }

        [MenuItem("Tools/Moonstone/Hierarchy Customization/Toggle Separator")]
        public static void ToggleSeparatorMenuItem()
        {
            data.enableSeparator = !data.enableSeparator;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem("Tools/Moonstone/Hierarchy Customization/Toggle Tree")]
        public static void ToggleTreeMenuItem()
        {
            data.enableTree = !data.enableTree;
            EditorApplication.RepaintHierarchyWindow();
        }

        static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (data == null) { return; }

            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }

            Separator separatorData = FindSeparatorDataByName(gameObject.name);
            bool isSeparator = separatorData != null;

            if (!isSeparator)
                HierarchyRenderer.RenderToggle(gameObject, selectionRect);

            if (data.enableSeparator && isSeparator)
                HierarchyRenderer.RenderSeparator(gameObject, selectionRect, separatorData);

            if (data.enableTree && !isSeparator)
                HierarchyRenderer.RenderTree(gameObject, selectionRect);
        }

        static Separator FindSeparatorDataByName(string name)
        {
            if (data == null || data.separatorList == null) { return null; }
            return Array.Find(data.separatorList, item => name.StartsWith(item.prefix));
        }
    }
}
#endif
