using UnityEngine;
using UnityEngine.Events;

namespace Moonstone.Core.Manager
{
    public enum ApplicationState
    {
        None,
        Initializing,
        Running,
        Paused,
        PendingExit,
        Exiting,
    }

    public class Application : ManagerBase
    {
        [Header("Managers")]
        [SerializeField] ManagerBase[] managers;

        [Header("Events")]
        [SerializeField] UnityEvent onInitializedEvent;
        [SerializeField] UnityEvent onStartUpEvent;
        [SerializeField] UnityEvent onQuitEvent;

        [Header("State")]
        public ApplicationState State;

        bool isQuitReserved;

        void Awake()
        {
            State = ApplicationState.Initializing;

            ServiceLocator.Register(this);

            foreach (var manager in managers)
            {
                ServiceLocator.Register(manager);
            }

            onInitializedEvent?.Invoke();
        }

        void Start()
        {
            enabled = false;

            SetUp();
        }

        void LateUpdate()
        {
            if (!isQuitReserved) { return; }

            enabled = false;
            isQuitReserved = false;

            QuitInternal();
        }

        void QuitInternal()
        {
            TearDown();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        public override void SetUp()
        {
            foreach (var manager in managers)
            {
                if (manager == null) { continue; }
                manager.SetUp();
            }

            State = ApplicationState.Running;
            onStartUpEvent?.Invoke();
        }

        public override void TearDown()
        {
            onQuitEvent?.Invoke();
            State = ApplicationState.Exiting;

            foreach (var manager in managers)
            {
                if (manager == null) { continue; }
                manager.TearDown();
            }
        }

        public void Quit()
        {
            isQuitReserved = true;
            enabled = true;

            State = ApplicationState.PendingExit;
        }
    }
}
