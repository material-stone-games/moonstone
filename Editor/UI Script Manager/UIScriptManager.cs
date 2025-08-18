using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace Moonstone.UIScriptManagement
{
    public class UIScriptManager : EditorWindow
    {
        const string SETTING_FILENAME = "UI Script Management";

        UIScriptManagementData settings;

        string applicationName;
        string outputPath;

        GameObject canvasObject;
        Vector2 hierarchyPreviewScrollPosition;
        Vector2 errorMessageScrollPosition;
        Dictionary<Transform, bool> foldoutStates = new();

        bool isGenerated = false;
        bool isAttached = false;

        bool isHierarchyValid = false;
        bool showErrorList = true;
        List<string> errorMessages = new();

        [MenuItem("Tools/Moonstone/UI Script Management")]
        public static void ShowWindow()
        {
            GetWindow<UIScriptManager>("UI Script Management");
        }

        void OnEnable()
        {
            LoadSettings();
            Refresh();

            EditorApplication.hierarchyChanged += Refresh;
        }

        void OnDisable()
        {
            EditorApplication.hierarchyChanged -= Refresh;
        }

        void LoadSettings()
        {
            var settingsPath = $"Packages/{Constants.PACKAGE_NAME}/Editor/Data/{SETTING_FILENAME}.asset";
            settings = AssetDatabase.LoadAssetAtPath<UIScriptManagementData>(settingsPath);

            if (settings == null)
            {
                settings = CreateInstance<UIScriptManagementData>();

                string folder = Path.GetDirectoryName(settingsPath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            applicationName = settings.ApplicationName;
            outputPath = settings.OutputPath;
        }

        void OnGUI()
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            applicationName = EditorGUILayout.TextField("Application Name", applicationName);
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);

            GUILayout.Space(10);

            GUILayout.Label("Hierarchy Preview", EditorStyles.boldLabel);
            canvasObject = (GameObject)EditorGUILayout.ObjectField("Canvas", canvasObject, typeof(GameObject), true);

            errorMessages.Clear();
            isHierarchyValid = true;

            GUILayout.BeginVertical("helpbox");
            if (canvasObject != null && canvasObject.GetComponent<Canvas>() != null)
            {
                hierarchyPreviewScrollPosition = EditorGUILayout.BeginScrollView(hierarchyPreviewScrollPosition, GUILayout.Height(100));

                foreach (Transform child in canvasObject.transform)
                {
                    DrawHierarchyTree(child, 0);
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label("⚠ Canvas를 선택해주세요.", labelStyle, GUILayout.Height(100));
                isHierarchyValid = false;
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);

            // Error List Foldout
            showErrorList = EditorGUILayout.Foldout(showErrorList, "Error List");
            if (showErrorList && errorMessages.Count > 0)
            {
                GUILayout.BeginVertical("helpbox");
                errorMessageScrollPosition = EditorGUILayout.BeginScrollView(errorMessageScrollPosition, GUILayout.Height(150));
                GUILayout.BeginVertical("box");
                foreach (var err in errorMessages)
                {
                    EditorGUILayout.LabelField(err, EditorStyles.wordWrappedLabel);
                }
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            GUILayout.Space(10);

            GUI.enabled = isHierarchyValid;
            if (GUILayout.Button("Generate Scripts"))
            {
                UIScriptSynchronizer.SyncScripts(canvasObject, applicationName, outputPath);
                Refresh();
            }
            GUI.enabled = true;

            GUI.enabled = isHierarchyValid && isGenerated;
            if (GUILayout.Button("Attach Scripts"))
            {
                UIScriptAttacher.AttachScripts(canvasObject, applicationName);
                Refresh();
            }
            GUI.enabled = true;

            GUI.enabled = isHierarchyValid && isAttached;
            if (GUILayout.Button("Detach Scripts"))
            {
                UIScriptDetacher.DetachScripts(canvasObject, applicationName);
                Refresh();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }
        }

        void DrawHierarchyTree(Transform obj, int indent)
        {
            if (!foldoutStates.ContainsKey(obj))
                foldoutStates[obj] = true;

            string error;
            bool valid = IsValidName(obj.name, out error);

            GUIStyle style = new GUIStyle(EditorStyles.label);
            if (!valid)
            {
                style.normal.textColor = Color.red;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(indent * 15);
            if (obj.childCount > 0)
                foldoutStates[obj] = EditorGUILayout.Foldout(foldoutStates[obj], obj.name, true);
            else
                GUILayout.Label(obj.name);
            if (!valid)
            {
                GUIStyle errorStyle = new GUIStyle(EditorStyles.label);
                errorStyle.alignment = TextAnchor.MiddleRight;
                errorStyle.normal.textColor = Color.red;
                GUILayout.Label($"⚠", errorStyle);
            }
            GUILayout.EndHorizontal();

            if (!valid) isHierarchyValid = false;
            if (!valid) errorMessages.Add(obj.name + ": " + error);

            // Screen 바로 밑까지만 표시
            if (foldoutStates[obj])
            {
                if (obj.parent != null && obj.parent == canvasObject.transform)
                {
                    foreach (Transform child in obj)
                    {
                        DrawHierarchyTree(child, indent + 1);
                    }
                }
            }
        }

        bool IsValidName(string name, out string error)
        {
            string sanitized = name.Replace(" ", "");

            if (string.IsNullOrEmpty(sanitized))
            {
                error = "이름이 비어있음";
                return false;
            }

            if (!Regex.IsMatch(sanitized, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                error = "C# 클래스명 규칙 위반 (영문자/숫자/언더스코어만 가능, 숫자로 시작 불가)";
                return false;
            }

            error = null;
            return true;
        }

        void Refresh()
        {
            settings.ApplicationName = applicationName;
            settings.OutputPath = outputPath;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (canvasObject == null) { return; }
            isGenerated = UIScriptGenerator.IsScriptsGenerated(canvasObject, applicationName);
            isAttached = UIScriptAttacher.IsScriptsAttached(canvasObject, applicationName);
        }
    }
}
