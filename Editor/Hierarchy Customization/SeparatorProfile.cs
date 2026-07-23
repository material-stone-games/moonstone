#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Moonstone.HierarchyCustomization
{
    [Serializable]
    public class SeparatorProfile
    {
        public string prefix = "#";
        public bool enableGradient;
        public GradientAlignment gradientAlignment;
        public Color backgroundColor = Color.black;
        public Font font;
        public FontStyle fontStyle = FontStyle.Normal;
        public Color fontColor = Color.white;
        public int fontSize = 12;
        public TextAnchor alignment = TextAnchor.MiddleLeft;
    }
}
#endif
