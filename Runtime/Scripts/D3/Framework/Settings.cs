using D3.Infrastructure;
using UnityEngine;

namespace Moonstone.D3.Framework
{
    [CreateAssetMenu(fileName = "D3Settings", menuName = "D3/Settings")]
    public class Settings : ScriptableObject
    {
        public Bootstrap BootstrapPrefab;
    }
}