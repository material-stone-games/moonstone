using UnityEngine;

namespace Moonstone.Arc.Framework
{
    [CreateAssetMenu(fileName = "Moonstone Arc Settings", menuName = "Moonstone/Arc/Settings")]
    public class ArcSettings : ScriptableObject
    {
        public bool Enabled;
        public Bootstrapper BootstrapperPrefab;
    }
}