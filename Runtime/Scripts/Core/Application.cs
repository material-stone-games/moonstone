using UnityEngine;
using UnityEngine.Events;

namespace Moonstone.Core
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

    public class Application : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] Manager.ManagerBase[] managers;

        [Header("Scenes")]
        [SerializeField] View.UI.Scene[] scenes;
        [SerializeField] bool activateFirstSceneOnStart = true;

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

        void SetUp()
        {
            foreach (var manager in managers)
            {
                if (manager == null) { continue; }
                manager.SetUp();
            }

            foreach (var scene in scenes)
            {
                if (scene == null) { continue; }
                scene.SetUp();
            }

            if (activateFirstSceneOnStart && scenes.Length > 0 && scenes[0] != null)
            {
                for (int i = 1; i < scenes.Length; i++) { scenes[i]?.Hide(); }
                scenes[0].Show();
            }

            State = ApplicationState.Running;
            onStartUpEvent?.Invoke();
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

        void TearDown()
        {
            onQuitEvent?.Invoke();
            State = ApplicationState.Exiting;

            foreach (var scene in scenes)
            {
                if (scene == null) { continue; }
                scene.TearDown();
            }

            foreach (var manager in managers)
            {
                if (manager == null) { continue; }
                manager.TearDown();
            }

            ServiceLocator.Clear();

            State = ApplicationState.None;
        }

        public void Quit()
        {
            isQuitReserved = true;
            enabled = true;

            State = ApplicationState.PendingExit;
        }

        void OnDestroy()
        {
            if (State != ApplicationState.Exiting)
            {
                TearDown();
            }
        }
    }
}
