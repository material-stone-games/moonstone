#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Moonstone.HierarchyCustomization
{
    public enum GradientAlignment { Left, Center, Right }

    [CreateAssetMenu(fileName = "HierarchyCustomizationData", menuName = "Moonstone/Hierarchy Customization")]
    public class HierarchyCustomizationData : ScriptableObject
    {
        public bool enableSeparator = true;
        public bool enableTree = true;

        [Serializable]
        public class Separator
        {
            public string prefix;
            public bool enableGradient;
            public GradientAlignment gradientAlignment;
            public Color backgroundColor = Color.black;
            public Font font;
            public FontStyle fontStyle = FontStyle.Normal;
            public Color fontColor = Color.white;
            public int fontSize = 12;
            public TextAnchor alignment = TextAnchor.MiddleLeft;
        }

        public Separator[] separatorList;

        void OnValidate()
        {
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
#endif
