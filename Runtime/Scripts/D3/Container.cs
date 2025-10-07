using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moonstone.D3.Framework;

namespace Moonstone.D3
{
    public class Container : IResolver
    {
        public static Container Instance { get; private set; }

        private readonly IEventDispatcher _eventDispatcher;
        private readonly Dictionary<Type, ServiceDescriptor> _services = new();

        public IEventDispatcher EventDispatcher => _eventDispatcher;

        public Container(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            Instance = this;
            _services[typeof(IEventDispatcher)] = new ServiceDescriptor(Lifetime.Singleton, _ => _eventDispatcher);
        }

        public void Register<TService, TImplementation>(params object[] constructorArgs) where TImplementation : TService
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);

            Lifetime lifetime;

            if (typeof(ITransientService).IsAssignableFrom(serviceType))
                lifetime = Lifetime.Transient;
            else if (typeof(ISingletonService).IsAssignableFrom(serviceType))
                lifetime = Lifetime.Singleton;
            else
                throw new InvalidOperationException($"Service type {serviceType} must implement either ITransientService or ISingletonService.");

            var descriptor = new ServiceDescriptor(lifetime, resolver =>
            {
                var constructor = implementationType.GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault();

                if (constructor == null)
                    return Activator.CreateInstance(implementationType);

                var parameters = constructor.GetParameters();
                var parameterInstances = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i < constructorArgs.Length && constructorArgs[i] != null && parameters[i].ParameterType.IsAssignableFrom(constructorArgs[i].GetType()))
                        parameterInstances[i] = constructorArgs[i];
                    else
                        parameterInstances[i] = resolver.Resolve(parameters[i].ParameterType);
                }

                return Activator.CreateInstance(implementationType, parameterInstances);
            });

            _services[serviceType] = descriptor;
        }

        public void Register<TService>(TService instance)
        {
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(Lifetime.Singleton, _ => instance);
            _services[serviceType] = descriptor;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var descriptor))
                return descriptor.GetInstance(this);

            throw new InvalidOperationException($"Service of type {serviceType} is not registered.");
        }

        public void BindEachOther()
        {
            foreach (var descriptor in _services.Values)
            {
                var instance = descriptor.GetInstance(this);
                if (descriptor.Lifetime != Lifetime.Singleton) continue;
                if (instance == null) continue;
                var type = instance.GetType();
                while (type != null)
                {
                    var privateFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    foreach (var field in privateFields)
                    {
                        if (_services.ContainsKey(field.FieldType))
                        {
                            var value = Resolve(field.FieldType);
                            field.SetValue(instance, value);
                        }
                    }
                    type = type.BaseType;
                }
            }
        }
    }
}