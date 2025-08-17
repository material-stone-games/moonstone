using UnityEngine;

namespace Moonstone.Core.System
{
    public class SystemBase : MonoBehaviour
    {
        protected virtual void Awake() => enabled = false;

        public virtual void Initialize() { }
        public virtual void Dispose() { }
        public virtual void SetUp() => enabled = true;
        public virtual void TearDown() => enabled = false;
    }
}