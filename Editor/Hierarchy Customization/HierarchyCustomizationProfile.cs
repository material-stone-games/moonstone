#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Moonstone.HierarchyCustomization
{
    public enum GradientAlignment { Left, Center, Right }

    [CreateAssetMenu(fileName = "Hierarchy Customization Profile", menuName = "Moonstone/Hierarchy Customization Profile")]
    public class HierarchyCustomizationProfile : ScriptableObject
    {
        public bool enableSeparator = true;
        public bool enableTree = true;

        [FormerlySerializedAs("separatorList")]
        public SeparatorProfile[] separators = {
            new() { prefix = "#1", backgroundColor = new Color(0.4f, 0.078125f, 0.078125f), alignment = TextAnchor.MiddleCenter },
            new() { prefix = "#2", backgroundColor = new Color(0.4f, 0.2375f, 0.078125f), alignment = TextAnchor.MiddleCenter },
            new() { prefix = "#3", backgroundColor = new Color(0.4f, 0.4f, 0.078125f), alignment = TextAnchor.MiddleCenter },
            new() { prefix = "#4", backgroundColor = new Color(0.07843137f, 0.4f, 0.07843137f), alignment = TextAnchor.MiddleCenter },
            new() { prefix = "#5", backgroundColor = new Color(0.07843137f, 0.23921569f, 0.4f), alignment = TextAnchor.MiddleCenter },
        };

        void OnValidate()
        {
            separators ??= Array.Empty<SeparatorProfile>();

            foreach (SeparatorProfile separator in separators)
            {
                if (separator == null) { continue; }

                separator.prefix ??= string.Empty;
                separator.fontSize = Math.Max(0, separator.fontSize);
            }

            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
#endif
