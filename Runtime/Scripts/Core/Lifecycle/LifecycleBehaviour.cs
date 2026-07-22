using System.Threading.Tasks;
using UnityEngine;

namespace Moonstone.Core
{
    public abstract class LifecycleBehaviour : MonoBehaviour, Moonstone.IInitializable, Moonstone.IAsyncInitializable, Moonstone.ILifecycleDisposable, System.IDisposable
    {
        public bool IsInitialized { get; private set; }
        public bool IsAsyncInitialized { get; private set; }
        public bool IsDisposed { get; private set; }

        public void Initialize()
        {
            if (IsInitialized) return;

            OnInitialize();
            IsInitialized = true;
        }

        public async Task InitializeAsync()
        {
            Initialize();

            if (IsAsyncInitialized) return;

            await OnInitializeAsync();
            IsAsyncInitialized = true;
        }

        public void Dispose()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            OnDispose();
        }

        protected virtual void OnInitialize() { }

        protected virtual Task OnInitializeAsync() => Task.CompletedTask;

        protected virtual void OnDispose() { }
    }
}
