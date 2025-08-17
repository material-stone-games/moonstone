#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Moonstone.HierarchyCustomization
{
    public static class HierarchyRenderer
    {
        static readonly Color BackgroundColor = new(0.2196079f, 0.2196079f, 0.2196079f, 1f);
        static readonly Dictionary<(Color, GradientAlignment), Texture2D> GradientTextureCache = new();

        public static void RenderToggle(GameObject gameObject, Rect selectionRect)
        {
            Rect toggleRect = new Rect(32, selectionRect.y, selectionRect.height, selectionRect.height);
            bool isActive = gameObject.activeSelf;
            bool isToggleActive = GUI.Toggle(toggleRect, isActive, string.Empty);
            if (isToggleActive == isActive) { return; }

            gameObject.SetActive(isToggleActive);

            if (EditorApplication.isPlaying) { return; }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            EditorUtility.SetDirty(gameObject);
        }

        public static void RenderSeparator(GameObject gameObject, Rect selectionRect, HierarchyCustomizationData.Separator customizedItemData)
        {
            if (customizedItemData == null) { return; }

            int prefixLength = customizedItemData.prefix.Length;

            if (prefixLength <= 0) { return; }
            if (prefixLength >= gameObject.name.Length) { return; }

            #region Render Background
            RenderEditorBackground(selectionRect);

            Color color = customizedItemData.backgroundColor;
            bool enableGradient = customizedItemData.enableGradient;
            var gradientAlignment = customizedItemData.gradientAlignment;

            RenderBackground(selectionRect, color, enableGradient, gradientAlignment);
            #endregion

            #region Render Text
            string text = gameObject.name[prefixLength..].Trim();
            Font font = customizedItemData.font;
            FontStyle fontStyle = customizedItemData.fontStyle;
            Color fontColor = customizedItemData.fontColor;
            int fontSize = customizedItemData.fontSize;
            TextAnchor alignment = customizedItemData.alignment;

            RenderText(selectionRect, text, font, fontStyle, fontColor, fontSize, alignment);
            #endregion
        }

        public static void RenderTree(GameObject gameObject, Rect selectionRect)
        {
            #region Render Tree
                RenderTree(selectionRect, gameObject);
            #endregion
        }

        static void RenderEditorBackground(Rect rect)
        {
            DrawRectangle(rect, BackgroundColor);
        }

        static void RenderBackground(Rect rect, Color color, bool enableGradient, GradientAlignment gradientAlignment)
        {
            if (enableGradient)
            {
                DrawGradient(rect, color, gradientAlignment);
            }
            else {
                DrawRectangle(rect, color);
            }
        }

        static void DrawRectangle(Rect rect, Color color)
        {
            EditorGUI.DrawRect(rect, color);
        }

        static void DrawGradient(Rect rect, Color color, GradientAlignment alignment)
        {
            if (GradientTextureCache.TryGetValue((color, alignment), out Texture2D texture))
            {
                GUI.DrawTexture(rect, texture);
                return;
            }

            Texture2D createdTexture2d = CreateColorTexture(rect, color, alignment);

            GradientTextureCache[(color, alignment)] = createdTexture2d;

            GUI.DrawTexture(rect, createdTexture2d);
        }

        static Texture2D CreateColorTexture(Rect rect, Color color, GradientAlignment alignment)
        {
            int width = (int) rect.width;
            int height = (int) rect.height;

            Texture2D texture2d = new Texture2D(width, height, TextureFormat.RGBA32, false);

            texture2d.hideFlags = HideFlags.HideAndDontSave;

            Color32[] array = new Color32[width * height];

            var transparentColor = new Color(color.r, color.g, color.b, 0);

            for (int i = 0; i < width; i++)
            {
                Color32 intermediateColor = alignment switch
                {
                    GradientAlignment.Center => Color.Lerp(transparentColor, color, Mathf.Sin(Mathf.PI * i / (width - 1))),
                    GradientAlignment.Left => Color.Lerp(color, transparentColor, (float)i / (width - 1)),
                    GradientAlignment.Right => Color.Lerp(transparentColor, color, (float)i / (width - 1)),
                    _ => Color.clear,
                };

                for (int j = 0; j < height; j++)
                {
                    array[j * width + i] = intermediateColor;
                }
            }

            texture2d.SetPixels32(array);
            texture2d.wrapMode = TextureWrapMode.Clamp;
            texture2d.Apply();

            return texture2d;
        }

        static void RenderText(Rect rect, string text, Font font, FontStyle fontStyle, Color fontColor, int fontSize, TextAnchor alignment)
        {
            DrawText(rect, text, font, fontStyle, fontColor, fontSize, alignment);
        }

        static void DrawText(Rect rect, string text, Font font, FontStyle fontStyle, Color fontColor, int fontSize, TextAnchor alignment)
        {
            GUI.Label(rect, text, new GUIStyle() {
                fontStyle = fontStyle,
                font = font,
                normal = new GUIStyleState() { textColor = fontColor },
                fontSize = fontSize,
                alignment = alignment,
            });
        }

        static void RenderTree(Rect rect, GameObject gameObject)
        {
            int leftSideBarWidth = 32;

            // depth icon size: 14x16
            int depthIconWidth = 14;
            Rect depthRect = new Rect() {
                x = leftSideBarWidth + depthIconWidth,
                y = rect.y,
                width = depthIconWidth,
                height = rect.height,
            };

            int depth = (int) (rect.x - leftSideBarWidth) / depthIconWidth - 2;

            for (int i = 0; i < depth; i++, depthRect.x += depthIconWidth)
            {
                DrawTreeDashLine(depthRect);
            }

            bool hasChildren = gameObject.transform.childCount > 0;
            bool isVirtualCamera = gameObject.transform.childCount == 1 && gameObject.transform.GetChild(0).name == "cm";
            if (!hasChildren || isVirtualCamera)
            {
                DrawTreeNodeLine(depthRect);
            }
        }

        static void DrawTreeDashLine(Rect rect)
        {
            Rect dotLine = new Rect() {
                x = rect.x + 6,
                y = rect.y,
                width = 2,
                height = 2,
            };

            for (int i = 0; i < 8; i += 2)
            {
                EditorGUI.DrawRect(dotLine, Color.gray);
                dotLine.y += 4;
            }
        }

        static void DrawTreeNodeLine(Rect rect)
        {
            Rect nodeLine1 = new Rect() {
                x = rect.x + 6,
                y = rect.y,
                width = 2,
                height = 16,
            };

            Rect nodeLine2 = new Rect() {
                x = rect.x + 6,
                y = rect.y + 7,
                width = 8,
                height = 2,
            };

            EditorGUI.DrawRect(nodeLine1, Color.gray);
            EditorGUI.DrawRect(nodeLine2, Color.gray);
        }
    }
}
#endif
