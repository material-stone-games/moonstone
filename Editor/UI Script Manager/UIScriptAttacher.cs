using System;
using UnityEngine;

namespace Moonstone.UIScriptManagement
{
    public static class UIScriptAttacher
    {
        public static void AttachScripts(GameObject canvasRoot, string appName)
        {
            AttachScriptsInternal(canvasRoot, appName);
            AssignScripts(canvasRoot, appName);
        }

        static void AttachScriptsInternal(GameObject canvasRoot, string appName)
        {
            foreach (Transform screenTransform in canvasRoot.transform)
            {
                string screenName = UIScriptNameSanitizer.Sanitize(screenTransform.name);
                AttachScriptToGameObject($"{appName}.View.UI.{screenName}.{screenName}Scene, Assembly-CSharp", screenTransform.gameObject);

                foreach (Transform viewTransform in screenTransform)
                {
                    string viewName = UIScriptNameSanitizer.Sanitize(viewTransform.name);
                    AttachScriptToGameObject($"{appName}.View.UI.{screenName}.{viewName}, Assembly-CSharp", viewTransform.gameObject);
                }
            }
        }

        static void AttachScriptToGameObject(string fullTypeName, GameObject obj)
        {
            Type type = Type.GetType(fullTypeName);
            if (type != null && obj != null)
            {
                if (obj.GetComponent(type) == null)
                {
                    obj.AddComponent(type);
                }
            }
        }

        static void AssignScripts(GameObject canvasRoot, string appName)
        {
            foreach (Transform screenTransform in canvasRoot.transform)
            {
                string screenName = UIScriptNameSanitizer.Sanitize(screenTransform.name);
                Type screenType = Type.GetType($"{appName}.View.UI.{screenName}.{screenName}Scene, Assembly-CSharp");
                if (screenType == null) continue;

                Component screenComp = screenTransform.gameObject.GetComponent(screenType);

                foreach (Transform viewTransform in screenTransform)
                {
                    string viewName = UIScriptNameSanitizer.Sanitize(viewTransform.name);
                    string variableName = $"{viewName}View";

                    // 자식 View 타입 가져오기
                    Type viewType = Type.GetType($"{appName}.View.UI.{screenName}.{viewName}, Assembly-CSharp");
                    if (viewType == null) continue;

                    // // 자식 오브젝트에 View 컴포넌트 추가
                    Component viewComp = viewTransform.gameObject.GetComponent(viewType);
                    if (viewComp == null)
                        viewComp = viewTransform.gameObject.AddComponent(viewType);

                    // Screen 변수에 할당
                    var field = screenComp.GetType().GetField(variableName);
                    if (field != null)
                        field.SetValue(screenComp, viewComp);
                }
            }
        }

        public static bool IsScriptsAttached(GameObject canvasRoot, string appName)
        {
            foreach (Transform screenTransform in canvasRoot.transform)
            {
                string screenName = UIScriptNameSanitizer.Sanitize(screenTransform.name);
                Type screenType = Type.GetType($"{appName}.View.UI.{screenName}.{screenName}Scene, Assembly-CSharp");
                if (screenType == null) return false;

                if (!screenTransform.TryGetComponent(screenType, out _))
                    return false;

                foreach (Transform viewTransform in screenTransform)
                {
                    string viewName = UIScriptNameSanitizer.Sanitize(viewTransform.name);
                    Type viewType = Type.GetType($"{appName}.View.UI.{screenName}.{viewName}, Assembly-CSharp");
                    if (viewType == null) continue;

                    if (!viewTransform.TryGetComponent(viewType, out _))
                        return false;
                }
            }

            return true;
        }
    }
}
