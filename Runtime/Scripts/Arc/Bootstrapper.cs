using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moonstone.Arc.Events;
using Moonstone.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moonstone.Arc
{
    public abstract class Bootstrapper : MonoBehaviour
    {
        private static Bootstrapper _active;

        [SerializeField] private bool persistentAcrossScenes = false;

        private GameObject[] _rootObjects = Array.Empty<GameObject>();
        private bool _isBootstrapped;
        private bool _isDisposing;

        private async void Start()
        {
            await BootstrapAsync();
        }

        private async Task BootstrapAsync()
        {
            if (_isBootstrapped) return;

            if (_active != null && _active != this)
            {
                Debug.LogWarning($"Arc bootstrapper already exists: {_active.name}. Skipping {name}.");
                return;
            }

            _active = this;
            _isBootstrapped = true;

            if (persistentAcrossScenes)
                DontDestroyOnLoad(gameObject);

            _rootObjects = GetSceneRootObjects();

            try
            {
                Configure();
                ConfigureDefaultServices();
                Container.BindEachOther();

                var sceneInjector = new DependencyInjection.SceneInjector(this, Container.Resolver);
                sceneInjector.Inject(_rootObjects);

                Initialize();
                await LifecycleRunner.InitializeHierarchyAsync(_rootObjects);

                await StartAsync();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private GameObject[] GetSceneRootObjects()
        {
            var roots = new List<GameObject>(SceneManager.GetActiveScene().GetRootGameObjects());
            if (!roots.Contains(gameObject))
                roots.Add(gameObject);

            return roots.ToArray();
        }

        protected virtual void Configure() { }

        private void ConfigureDefaultServices()
        {
            if (!Container.TryResolve<IEventBus>(out _))
                Container.Register<IEventBus>(new EventBus());
        }

        protected virtual void Initialize() { }

        protected virtual Task StartAsync() => Task.CompletedTask;

        private void OnDestroy()
        {
            if (_active != this || _isDisposing) return;

            _isDisposing = true;

            try
            {
                LifecycleRunner.DisposeHierarchy(_rootObjects);
                Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                Container.Dispose();
                _active = null;
                _isBootstrapped = false;
                _isDisposing = false;
            }
        }

        protected virtual void Dispose() { }
    }
}
