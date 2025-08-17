using UnityEngine;

namespace Moonstone
{
    public class Launcher : MonoBehaviour
    {
        void Awake()
        {
            OnLauncherInitialized();
        }

        void Start()
        {
            OnLauncherStarted();
        }

        protected virtual void OnLauncherInitialized() { }
        protected virtual void OnLauncherStarted() { }
    }
}