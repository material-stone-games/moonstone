#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Moonstone
{
    public class SetupWindow : EditorWindow
    {
        [MenuItem("Window/MaterialStone/Project Setup")]
        public static void ShowWindow()
        {
            GetWindow<SetupWindow>("Project Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("프로젝트 템플릿 셋업", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("템플릿 적용하기"))
            {
                ApplyTemplate();
            }
        }

        private void ApplyTemplate()
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Constants.PACKAGE_NAME);
            if (packageInfo == null)
            {
                Debug.LogError($"패키지를 찾을 수 없습니다: {Constants.PACKAGE_NAME}");
                return;
            }

            string projectStructurePath = Path.Combine(packageInfo.resolvedPath, "Templates", "ProjectStructure");
            if (!Directory.Exists(projectStructurePath))
            {
                Debug.LogError($"프로젝트 구조 폴더가 존재하지 않습니다: {projectStructurePath}");
                return;
            }

            string targetPath = Path.Combine(Application.dataPath);

            CopyDirectory(projectStructurePath, targetPath);
            AssetDatabase.Refresh();

            Debug.Log("템플릿 셋업 완료!");
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
