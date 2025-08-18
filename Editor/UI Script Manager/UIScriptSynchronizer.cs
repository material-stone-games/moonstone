using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Moonstone.UIScriptManagement
{
    public static class UIScriptSynchronizer
    {
        public static void SyncScripts(GameObject canvasRoot, string appName, string targetPath)
        {
            // 1. 기대 스크립트 집합 생성
            var expectedFiles = UIScriptGenerator.CollectExpectedScripts(canvasRoot, appName, targetPath);

            // 2. 실제 파일 목록
            string[] existingFiles = Directory.GetFiles(targetPath, "*.cs", SearchOption.AllDirectories);

            HashSet<string> processed = new HashSet<string>();

            // 3. 비교 및 업데이트
            foreach (var kvp in expectedFiles)
            {
                string filePath = kvp.Key;
                string content = kvp.Value;

                processed.Add(filePath);

                if (!File.Exists(filePath))
                {
                    // 새로 생성
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllText(filePath, content);
                    Debug.Log($"[UIScriptSync] Created: {filePath}");
                }
                else
                {
                    string oldContent = File.ReadAllText(filePath);
                    if (oldContent != content)
                    {
                        // 수정 필요
                        File.WriteAllText(filePath, content);
                        Debug.Log($"[UIScriptSync] Updated: {filePath}");
                    }
                }
            }

            // 4. 필요 없는 파일 삭제
            foreach (string file in existingFiles)
            {
                if (!processed.Contains(file))
                {
                    File.Delete(file);
                    File.Delete(file + ".meta"); // 메타 파일도 삭제
                    Debug.Log($"[UIScriptSync] Deleted: {file}");
                }
            }

            // 4-2. 빈 디렉토리 삭제
            string[] directories = Directory.GetDirectories(targetPath, "*", SearchOption.AllDirectories);
            foreach (string dir in directories)
            {
                if (Directory.GetFiles(dir).Length == 0)
                {
                    Directory.Delete(dir);
                    File.Delete(dir + ".meta"); // 메타 파일도 삭제
                    Debug.Log($"[UIScriptSync] Deleted empty directory: {dir}");
                }
            }

            // 5. 리프레시
            AssetDatabase.Refresh();
        }
    }
}
