using System.Threading.Tasks;
using UnityEngine;

namespace Moonstone.Arc
{
    public class ArcSettingsNotFoundException : System.Exception
    {
        public ArcSettingsNotFoundException() : base("ArcSettings ScriptableObject not found in the project.") { }
    }
    public class BootstrapperPrefabNotFoundException : System.Exception
    {
        public BootstrapperPrefabNotFoundException() : base("BootstrapperPrefab is not set in ArcSettings.") { }
    }

    public abstract class Bootstrapper : MonoBehaviour
    {
        private static bool _isInstalled = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            try
            {
                _isInstalled = false;

                var arcSettings = FindArcSettingsAsset();

                if (!arcSettings.Enabled) return;
                ValidateBootstrapperPrefab(arcSettings);

                var bootstrapper = CreateBootstrapper(arcSettings);
                InstallBootstrapper(bootstrapper);

                _isInstalled = true;
            }
            catch (ArcSettingsNotFoundException ex)
            {
                Debug.LogException(ex);
            }
            catch (BootstrapperPrefabNotFoundException ex)
            {
                Debug.LogException(ex);
            }
        }

        private static Framework.ArcSettings FindArcSettingsAsset()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:ArcSettings");
            if (guids.Length > 0)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<Framework.ArcSettings>(path);

                if (settings != null)
                    return settings;
            }

            throw new ArcSettingsNotFoundException();
        }

        private static void ValidateBootstrapperPrefab(Framework.ArcSettings settings)
        {
            if (settings.BootstrapperPrefab == null)
                throw new BootstrapperPrefabNotFoundException();
        }

        private static Bootstrapper CreateBootstrapper(Framework.ArcSettings settings)
        {
            var bootstrapper = Instantiate(settings.BootstrapperPrefab);
            DontDestroyOnLoad(bootstrapper);
            return bootstrapper;
        }

        private static void InstallBootstrapper(Bootstrapper bootstrapper)
        {
            var installer = new Framework.Installer(bootstrapper);
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            installer.Install(rootObjects);
        }

        private async void Start()
        {
            if (!_isInstalled) return;

            BindObjects();
            Container.BindEachOther();
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

            EventDispatcher.Dispose();
            Container.Dispose();
        }

        protected abstract void DisposeObjects();
    }
}