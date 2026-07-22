namespace Moonstone.Arc.DependencyInjection
{
    internal sealed class ServiceDescriptor
    {
        public object Instance { get; }

        public ServiceDescriptor(object singletonInstance)
        {
            Instance = singletonInstance;
        }

        public bool IsInjected { get; private set; }

        public void MarkInjected() => IsInjected = true;

        public void DisposeInstance(System.Collections.Generic.HashSet<object> disposed)
        {
            if (Instance == null || !disposed.Add(Instance)) return;

            if (Instance is Moonstone.ILifecycleDisposable lifecycleDisposable)
            {
                lifecycleDisposable.Dispose();
            }
            else if (Instance is System.IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
