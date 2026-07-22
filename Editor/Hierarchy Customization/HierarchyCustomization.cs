#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Moonstone.HierarchyCustomization
{
    using SeparatorProfile = HierarchyCustomizationProfile.SeparatorProfile;

    [InitializeOnLoad]
    public static class HierarchyCustomization
    {
        const string SETTING_FILENAME = "Hierarchy Customization";
        const string PROFILE_DIRECTORY = "Assets/Settings/Profiles";
        const string PROFILE_PATH = PROFILE_DIRECTORY + "/" + SETTING_FILENAME + ".asset";

        static HierarchyCustomizationProfile profile;

        static HierarchyCustomization()
        {
            profile = LoadProfile();

            EditorApplication.hierarchyWindowItemOnGUI -= HandleHierarchyWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
        }

        static HierarchyCustomizationProfile LoadProfile()
        {
            var loadedProfile = AssetDatabase.LoadAssetAtPath<HierarchyCustomizationProfile>(PROFILE_PATH);
            if (loadedProfile != null)
            {
                return loadedProfile;
            }

            return FindProfileInProject();
        }

        static HierarchyCustomizationProfile FindProfileInProject()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(HierarchyCustomizationProfile)}", new[] { "Assets" });
            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            Array.Sort(paths, StringComparer.OrdinalIgnoreCase);

            foreach (string path in paths)
            {
                var foundProfile = AssetDatabase.LoadAssetAtPath<HierarchyCustomizationProfile>(path);
                if (foundProfile != null)
                {
                    return foundProfile;
                }
            }

            return null;
        }

        static HierarchyCustomizationProfile CreateProfile()
        {
            EnsureDirectory(PROFILE_DIRECTORY);

            var createdProfile = ScriptableObject.CreateInstance<HierarchyCustomizationProfile>();
            AssetDatabase.CreateAsset(createdProfile, PROFILE_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath<HierarchyCustomizationProfile>(PROFILE_PATH);
        }

        static HierarchyCustomizationProfile LoadOrCreateProfileWithPrompt()
        {
            var loadedProfile = LoadProfile();
            if (loadedProfile != null)
            {
                return loadedProfile;
            }

            bool createProfile = EditorUtility.DisplayDialog(
                "Create Hierarchy Customization Profile",
                $"Hierarchy customization profile does not exist. Create it at {PROFILE_DIRECTORY}?",
                "Create",
                "Cancel");

            return createProfile ? CreateProfile() : null;
        }

        static void EnsureDirectory(string directory)
        {
            if (System.IO.Directory.Exists(directory)) { return; }

            string parent = System.IO.Path.GetDirectoryName(directory);
            if (!string.IsNullOrEmpty(parent))
            {
                EnsureDirectory(parent);
            }

            System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Moonstone/Hierarchy Customization/Edit Profile")]
        public static void EditProfileMenuItem()
        {
            profile = LoadOrCreateProfileWithPrompt();
            if (profile == null) { return; }

            Selection.activeObject = profile;
            EditorGUIUtility.PingObject(profile);
        }

        [MenuItem("Tools/Moonstone/Hierarchy Customization/Toggle Separator")]
        public static void ToggleSeparatorMenuItem()
        {
            profile = LoadOrCreateProfileWithPrompt();
            if (profile == null) { return; }

            profile.enableSeparator = !profile.enableSeparator;
            SaveProfile();
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem("Tools/Moonstone/Hierarchy Customization/Toggle Tree")]
        public static void ToggleTreeMenuItem()
        {
            profile = LoadOrCreateProfileWithPrompt();
            if (profile == null) { return; }

            profile.enableTree = !profile.enableTree;
            SaveProfile();
            EditorApplication.RepaintHierarchyWindow();
        }

        static void SaveProfile()
        {
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
        }

        static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (profile == null) { return; }

            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null) { return; }

            SeparatorProfile separatorProfile = FindSeparatorProfileByName(gameObject.name);
            bool isSeparator = separatorProfile != null;

            if (!isSeparator)
                HierarchyRenderer.RenderToggle(gameObject, selectionRect);

            if (profile.enableSeparator && isSeparator)
                HierarchyRenderer.RenderSeparator(gameObject, selectionRect, separatorProfile);

            if (profile.enableTree && !isSeparator)
                HierarchyRenderer.RenderTree(gameObject, selectionRect);
        }

        static SeparatorProfile FindSeparatorProfileByName(string name)
        {
            if (profile == null || profile.separators == null) { return null; }

            SeparatorProfile matchingProfile = null;
            foreach (SeparatorProfile separatorProfile in profile.separators)
            {
                if (separatorProfile == null || string.IsNullOrEmpty(separatorProfile.prefix)) { continue; }
                if (!name.StartsWith(separatorProfile.prefix, StringComparison.Ordinal)) { continue; }

                if (matchingProfile == null || separatorProfile.prefix.Length > matchingProfile.prefix.Length)
                {
                    matchingProfile = separatorProfile;
                }
            }

            return matchingProfile;
        }
    }
}
#endif
