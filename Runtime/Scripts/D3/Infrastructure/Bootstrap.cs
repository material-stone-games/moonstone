using System.Threading.Tasks;
using UnityEngine;

namespace Moonstone.D3.Infrastructure
{
    public abstract class Bootstrap : MonoBehaviour
    {
        protected Container container;

        private async void Start()
        {
            BindObjects();
            InitializeObjects();
            await CreateObjects();
            PrepareObjects();
        }

        protected abstract void BindObjects();
        protected abstract void InitializeObjects();
        protected abstract Task CreateObjects();
        protected abstract void PrepareObjects();

        private void OnDestroy()
        {
            DisposeObjects();
        }

        protected abstract void DisposeObjects();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:D3.Settings");
            if (guids.Length > 0)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<Framework.Settings>(path);
                if (settings == null || settings.BootstrapPrefab == null)
                {
                    Debug.LogWarning("D3Settings or bootstrapPrefab is not set.");
                    return;
                }

                var eventDispatcher = new Events.EventDispatcher();
                var container = new Container(eventDispatcher);

                var bootstrap = Instantiate(settings.BootstrapPrefab);
                bootstrap.container = container;
                DontDestroyOnLoad(bootstrap);

                var installer = new Framework.Installer(eventDispatcher, container, bootstrap);

                var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                installer.Install(rootObjects);
            }
            else
            {
                Debug.LogWarning("D3Settings ScriptableObject not found in the project.");
            }
        }
    }
}