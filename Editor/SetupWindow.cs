#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Moonstone
{
    public class SetupWindow : EditorWindow
    {
        Texture2D icon;
        string windowName = "Moonstone";

        [MenuItem("Window/Moonstone/Project Setup")]
        public static void ShowWindow()
        {
            GetWindow<SetupWindow>("Project Setup");
        }

        void OnEnable() {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"Packages/{Constants.PACKAGE_NAME}/Editor/Data/icon.png");
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height(100), GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            // 아이콘
            if (icon != null)
            {
                GUIStyle centeredIconStyle = new GUIStyle();
                centeredIconStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(icon, centeredIconStyle, GUILayout.Width(64), GUILayout.Height(64));
            }

            // 이름
            GUIStyle nameStyle = new GUIStyle(EditorStyles.label);
            nameStyle.fontSize = 24;
            nameStyle.alignment = TextAnchor.MiddleCenter;
            nameStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label(windowName, nameStyle, GUILayout.Height(64));

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label("Project Structure Template", EditorStyles.boldLabel);
            GUILayout.Label("This template sets up a basic project structure within the Assets folder.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                ApplyTemplate();
            }
        }

        void ApplyTemplate()
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Constants.PACKAGE_NAME);
            if (packageInfo == null)
            {
                Debug.LogError($"❌ Package not found: {Constants.PACKAGE_NAME}");
                return;
            }

            string projectStructurePath = Path.Combine(packageInfo.resolvedPath, "Templates", "ProjectStructure");
            if (!Directory.Exists(projectStructurePath))
            {
                Debug.LogError($"❌ Project structure folder does not exist: {projectStructurePath}");
                return;
            }

            string targetPath = Path.Combine(Application.dataPath);

            CopyDirectory(projectStructurePath, targetPath);
            AssetDatabase.Refresh();

            Debug.Log("✅ Complete applying project structure template!");
        }

        void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(targetDir, dirName);
                CopyDirectory(directory, destDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".meta") || fileName.Equals(".gitkeep")) { continue; }
                string destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, true);
            }
        }
    }
}
#endif
