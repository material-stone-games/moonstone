#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Moonstone
{
    public class SetupWindow : EditorWindow
    {
        [MenuItem("Window/Moonstone/Project Setup")]
        public static void ShowWindow()
        {
            GetWindow<SetupWindow>("Project Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Project Structure Template", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                ApplyTemplate();
            }
        }

        private void ApplyTemplate()
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Constants.PACKAGE_NAME);
            if (packageInfo == null)
            {
                Debug.LogError($"Package not found: {Constants.PACKAGE_NAME}");
                return;
            }

            string projectStructurePath = Path.Combine(packageInfo.resolvedPath, "Templates", "ProjectStructure");
            if (!Directory.Exists(projectStructurePath))
            {
                Debug.LogError($"Project structure folder does not exist: {projectStructurePath}");
                return;
            }

            string targetPath = Path.Combine(Application.dataPath);

            CopyDirectory(projectStructurePath, targetPath);
            AssetDatabase.Refresh();

            Debug.Log("Complete applying project structure template!");
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(targetDir, dirName);
                CopyDirectory(directory, destDir);
            }
        }
    }
}
#endif
