using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Moonstone.UIScriptManagement
{
    public static class UIScriptGenerator
    {
        public static Dictionary<string, string> CollectExpectedScripts(GameObject canvasRoot, string appName, string targetPath)
        {
            var dict = new Dictionary<string, string>();

            foreach (Transform screen in canvasRoot.transform)
            {
                string screenName = screen.name.Replace(" ", "");
                string screenFolder = Path.Combine(targetPath, screenName);
                string screenFile = Path.Combine(screenFolder, "Screen.cs");

                // 1-depth Screen
                string screenCode =
    $@"namespace {appName}.View.UI.{screenName}
{{
    public class Screen : Moonstone.View.UI.Screen
    {{
{GenerateChildFields(screen)}
    }}
}}";

                dict[screenFile] = screenCode;

                // 2-depth Views
                foreach (Transform child in screen)
                {
                    string viewName = child.name.Replace(" ", "");
                    string viewFile = Path.Combine(screenFolder, viewName + ".cs");

                    string viewCode =
    $@"namespace {appName}.View.UI.{screenName}
{{
    public class {viewName} : Moonstone.View.UI.View {{

    }}
}}";
                    dict[viewFile] = viewCode;
                }
            }

            return dict;
        }

        static string GenerateChildFields(Transform screen)
        {
            string result = "";
            foreach (Transform child in screen)
            {
                string safeName = child.name.Replace(" ", "");
                result += $"        public {safeName} {safeName}View;\n";
            }
            return result;
        }

        public static bool IsScriptsGenerated(GameObject canvasRoot, string appName)
        {
            foreach (Transform screenTransform in canvasRoot.transform)
            {
                var screenName = UIScriptNameSanitizer.Sanitize(screenTransform.name);

                Type screenType = Type.GetType($"{appName}.View.UI.{screenName}.Screen, Assembly-CSharp");
                if (screenType == null) return false;

                foreach (Transform viewTransform in screenTransform)
                {
                    string viewName = UIScriptNameSanitizer.Sanitize(viewTransform.name);

                    Type viewType = Type.GetType($"{appName}.View.UI.{screenName}.{viewName}, Assembly-CSharp");
                    if (viewType == null) return false;
                }
            }

            return true;
        }
    }
}
