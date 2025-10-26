using System;
using UnityEngine;

namespace Moonstone.UIScriptManagement
{
    public static class UIScriptDetacher
    {
        public static void DetachScripts(GameObject canvasRoot, string appName)
        {
            foreach (Transform screenTransform in canvasRoot.transform)
            {
                string screenName = UIScriptNameSanitizer.Sanitize(screenTransform.name);
                Type screenType = Type.GetType($"{appName}.View.UI.{screenName}.{screenName}Scene, Assembly-CSharp");
                if (screenType == null) continue;

                Component screenComp = screenTransform.gameObject.GetComponent(screenType);
                if (screenComp == null) continue;

                foreach (Transform viewTransform in screenTransform)
                {
                    string viewName = UIScriptNameSanitizer.Sanitize(viewTransform.name);
                    string variableName = $"{viewName}View";

                    // 자식 View 타입 가져오기
                    Type viewType = Type.GetType($"{appName}.View.UI.{screenName}.{viewName}, Assembly-CSharp");
                    if (viewType == null) continue;

                    // Screen 변수에서 View 컴포넌트 제거
                    var field = screenComp.GetType().GetField(variableName);
                    if (field != null)
                        field.SetValue(screenComp, null);

                    // 자식 오브젝트에서 View 컴포넌트 제거
                    Component viewComp = viewTransform.gameObject.GetComponent(viewType);
                    if (viewComp != null)
                        UnityEngine.Object.DestroyImmediate(viewComp);
                }

                if (screenComp != null)
                    UnityEngine.Object.DestroyImmediate(screenComp);
            }
        }
    }
}
