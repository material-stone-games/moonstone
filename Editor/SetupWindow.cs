#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moonstone
{
    public class SetupWindow : EditorWindow
    {
        const int MaxConflictPreviewCount = 10;

        Texture2D icon;
        string windowName = "Moonstone";
        string statusMessage;
        MessageType statusType = MessageType.Info;

        [MenuItem("Tools/Moonstone/Project Setup")]
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

            GUILayout.Space(16);
            GUILayout.Label("Scene Hierarchy Template", EditorStyles.boldLabel);
            GUILayout.Label("Adds Core and UI hierarchy groups to the currently open scene.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Create Scene Hierarchy"))
            {
                CreateSceneHierarchy();
            }

            if (!string.IsNullOrEmpty(statusMessage))
            {
                GUILayout.Space(16);
                EditorGUILayout.HelpBox(statusMessage, statusType);
            }
        }

        void ApplyTemplate()
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForPackageName(Constants.PACKAGE_NAME);
            if (packageInfo == null)
            {
                SetStatus($"Package not found: {Constants.PACKAGE_NAME}", MessageType.Error);
                Debug.LogError($"Package not found: {Constants.PACKAGE_NAME}");
                return;
            }

            string projectStructurePath = Path.Combine(packageInfo.resolvedPath, "Templates", "ProjectStructure");
            if (!Directory.Exists(projectStructurePath))
            {
                SetStatus($"Project structure folder does not exist: {projectStructurePath}", MessageType.Error);
                Debug.LogError($"Project structure folder does not exist: {projectStructurePath}");
                return;
            }

            string targetPath = Application.dataPath;
            List<string> conflicts = FindConflicts(projectStructurePath, targetPath);
            if (conflicts.Count > 0 && !ConfirmOverwrite(conflicts))
            {
                SetStatus("Project structure template was canceled. Existing files were not changed.", MessageType.Warning);
                return;
            }

            var rollback = new FileRollback();
            try
            {
                CopyDirectory(projectStructurePath, targetPath, rollback);
                rollback.CleanupBackups();
                AssetDatabase.Refresh();

                SetStatus(
                    $"Project structure template applied.\nCreated files: {rollback.CreatedFileCount}\nOverwritten files: {rollback.OverwrittenFileCount}\nCreated folders: {rollback.CreatedDirectoryCount}",
                    MessageType.Info);
                Debug.Log("Complete applying project structure template.");
            }
            catch (Exception exception)
            {
                rollback.Rollback();
                AssetDatabase.Refresh();

                SetStatus($"Failed to apply project structure template. Changes were rolled back.\n{exception.Message}", MessageType.Error);
                Debug.LogError(exception);
            }
        }

        void CopyDirectory(string sourceDir, string targetDir, FileRollback rollback)
        {
            rollback.EnsureDirectory(targetDir);

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(targetDir, dirName);
                CopyDirectory(directory, destDir, rollback);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                if (ShouldSkipTemplateFile(fileName)) { continue; }
                string destFile = Path.Combine(targetDir, fileName);
                rollback.CopyFile(file, destFile);
            }
        }

        void CreateSceneHierarchy()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid())
            {
                SetStatus("Could not add scene hierarchy. No valid scene is open.", MessageType.Error);
                return;
            }

            List<string> existingRoots = FindExistingRootObjects("Core", "UI");
            if (existingRoots.Count > 0 && !ConfirmDuplicateHierarchy(existingRoots))
            {
                SetStatus("Scene hierarchy creation was canceled. Current scene was not changed.", MessageType.Warning);
                return;
            }

            var createdObjects = new List<GameObject>();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create Moonstone Scene Hierarchy");

            try
            {
                CreateTemplateHierarchy(createdObjects);
                EditorSceneManager.MarkSceneDirty(activeScene);
                Undo.CollapseUndoOperations(undoGroup);

                SetStatus(
                    "Scene hierarchy created in the current scene.\nHierarchy: Core, UI",
                    MessageType.Info);
                Debug.Log("Scene hierarchy created in the current scene.");
            }
            catch (Exception exception)
            {
                for (int i = createdObjects.Count - 1; i >= 0; i--)
                {
                    if (createdObjects[i] != null)
                    {
                        DestroyImmediate(createdObjects[i]);
                    }
                }

                SetStatus($"Failed to create scene hierarchy. Created objects were removed.\n{exception.Message}", MessageType.Error);
                Debug.LogError(exception);
            }
        }

        void CreateTemplateHierarchy(List<GameObject> createdObjects)
        {
            GameObject core = CreateRoot("Core", createdObjects);
            CreateChild(core.transform, "Bootstrapper", createdObjects);
            CreateChild(core.transform, "Systems", createdObjects);
            CreateChild(core.transform, "Services", createdObjects);

            GameObject ui = CreateRoot("UI", createdObjects);
            GameObject canvas = CreateChild(ui.transform, "Canvas", createdObjects);
            CreateOptionalComponent(canvas, "UnityEngine.Canvas, UnityEngine.UIModule");
            CreateOptionalComponent(canvas, "UnityEngine.UI.CanvasScaler, UnityEngine.UI");
            CreateOptionalComponent(canvas, "UnityEngine.UI.GraphicRaycaster, UnityEngine.UI");
            CreateChild(canvas.transform, "Screens", createdObjects);
            CreateChild(canvas.transform, "Popups", createdObjects);
            CreateChild(canvas.transform, "Overlay", createdObjects);

            GameObject eventSystem = CreateChild(ui.transform, "EventSystem", createdObjects);
            CreateOptionalComponent(eventSystem, "UnityEngine.EventSystems.EventSystem, UnityEngine.UI");
            CreateOptionalComponent(eventSystem, "UnityEngine.EventSystems.StandaloneInputModule, UnityEngine.UI");
        }

        GameObject CreateRoot(string name, List<GameObject> createdObjects)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            RegisterCreatedObject(gameObject, createdObjects);
            return gameObject;
        }

        GameObject CreateChild(Transform parent, string name, List<GameObject> createdObjects, params Type[] componentTypes)
        {
            GameObject gameObject = new GameObject(name, componentTypes);
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            RegisterCreatedObject(gameObject, createdObjects);
            return gameObject;
        }

        void RegisterCreatedObject(GameObject gameObject, List<GameObject> createdObjects)
        {
            createdObjects.Add(gameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create Moonstone Scene Hierarchy");
        }

        void CreateOptionalComponent(GameObject gameObject, string typeName)
        {
            Type componentType = Type.GetType(typeName);
            if (componentType == null || !typeof(Component).IsAssignableFrom(componentType))
            {
                return;
            }

            if (gameObject.GetComponent(componentType) == null)
            {
                gameObject.AddComponent(componentType);
            }
        }

        List<string> FindConflicts(string sourceDir, string targetDir)
        {
            var conflicts = new List<string>();

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                if (ShouldSkipTemplateFile(fileName)) { continue; }

                string destFile = Path.Combine(targetDir, fileName);
                if (File.Exists(destFile))
                {
                    conflicts.Add(GetProjectRelativePath(destFile));
                }
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(directory);
                string destDir = Path.Combine(targetDir, dirName);
                conflicts.AddRange(FindConflicts(directory, destDir));
            }

            return conflicts;
        }

        bool ConfirmOverwrite(List<string> conflicts)
        {
            string message = $"The project structure template will overwrite {conflicts.Count} existing file(s).\n\n";
            int previewCount = Mathf.Min(conflicts.Count, MaxConflictPreviewCount);
            for (int i = 0; i < previewCount; i++)
            {
                message += $"- {conflicts[i]}\n";
            }

            if (conflicts.Count > previewCount)
            {
                message += $"- ...and {conflicts.Count - previewCount} more\n";
            }

            message += "\nContinue?";
            return EditorUtility.DisplayDialog("Overwrite Existing Files?", message, "Overwrite", "Cancel");
        }

        List<string> FindExistingRootObjects(params string[] rootNames)
        {
            var existingRoots = new List<string>();
            foreach (GameObject rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                for (int i = 0; i < rootNames.Length; i++)
                {
                    if (rootGameObject.name == rootNames[i])
                    {
                        existingRoots.Add(rootGameObject.name);
                        break;
                    }
                }
            }

            return existingRoots;
        }

        bool ConfirmDuplicateHierarchy(List<string> existingRoots)
        {
            string message = "The current scene already contains these root object(s):\n\n";
            for (int i = 0; i < existingRoots.Count; i++)
            {
                message += $"- {existingRoots[i]}\n";
            }

            message += "\nCreate another hierarchy anyway?";
            return EditorUtility.DisplayDialog("Create Duplicate Hierarchy?", message, "Create Anyway", "Cancel");
        }

        bool ShouldSkipTemplateFile(string fileName)
        {
            return fileName.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) ||
                fileName.Equals(".gitkeep", StringComparison.Ordinal);
        }

        string GetProjectRelativePath(string absolutePath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string relativePath = absolutePath.Substring(projectRoot.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relativePath.Replace(Path.DirectorySeparatorChar, '/');
        }

        void SetStatus(string message, MessageType type)
        {
            statusMessage = message;
            statusType = type;
            Repaint();
        }
    }
}
#endif
