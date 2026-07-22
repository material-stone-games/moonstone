using System;
using System.Collections.Generic;

namespace Moonstone.Arc.DependencyInjection
{
    public class ServiceResolver : IResolver, IDisposable
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new();
        private readonly List<Type> _injecting = new();
        private bool _isDisposing;

        public void Register<TService>(TService instance) where TService : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            Register(typeof(TService), instance);
        }

        public void Register(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (!serviceType.IsAssignableFrom(instance.GetType()))
                throw new InvalidOperationException($"{instance.GetType()} is not assignable to {serviceType}.");

            _services[serviceType] = new ServiceDescriptor(instance);
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            if (!_services.TryGetValue(serviceType, out var descriptor))
                throw new InvalidOperationException($"Service of type {serviceType} is not registered.");

            return descriptor.Instance;
        }

        public bool TryResolve<T>(out T service)
        {
            if (TryResolve(typeof(T), out var resolved))
            {
                service = (T)resolved;
                return true;
            }

            service = default;
            return false;
        }

        public bool TryResolve(Type serviceType, out object service)
        {
            if (serviceType == null || !_services.TryGetValue(serviceType, out var descriptor))
            {
                service = null;
                return false;
            }

            service = descriptor.Instance;
            return true;
        }

        public void BindEachOther()
        {
            foreach (var serviceType in new List<Type>(_services.Keys))
                InjectIfNeeded(serviceType);
        }

        public void Dispose()
        {
            if (_isDisposing) return;

            _isDisposing = true;
            try
            {
                var disposed = new HashSet<object>();
                foreach (var descriptor in _services.Values)
                    descriptor.DisposeInstance(disposed);

                _services.Clear();
                _injecting.Clear();
            }
            finally
            {
                _isDisposing = false;
            }
        }

        private void InjectIfNeeded(Type serviceType)
        {
            var descriptor = _services[serviceType];
            if (descriptor.IsInjected)
                return;

            if (_injecting.Contains(serviceType))
            {
                throw new InvalidOperationException($"Recursive injection detected for {serviceType}.");
            }

            _injecting.Add(serviceType);
            try
            {
                DependencyInjector.Inject(descriptor.Instance, this);
                descriptor.MarkInjected();
            }
            finally
            {
                _injecting.RemoveAt(_injecting.Count - 1);
            }
        }
    }
}
