using UnityEngine;

namespace Moonstone.Core.Manager
{
    public abstract class ManagerBase : MonoBehaviour
    {
        public virtual void SetUp() { }
        public virtual void TearDown() { }
    }
}