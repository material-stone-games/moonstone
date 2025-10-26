using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moonstone.D3.Presentation;

namespace Moonstone.Arc.Framework
{
    public class ServiceResolver : IResolver
    {
        private readonly Dictionary<Type, ServiceDescriptor> _services = new();

        public void Register<TImplementation>(params object[] constructorArgs) where TImplementation : class
        {
            var implementationType = typeof(TImplementation);

            var descriptor = new ServiceDescriptor(resolver =>
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

            _services[implementationType] = descriptor;
        }

        public void Register<TService>(TService instance)
        {
            var serviceType = typeof(TService);
            var descriptor = new ServiceDescriptor(_ => instance);
            _services[serviceType] = descriptor;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type serviceType)
        {
            if (_services.TryGetValue(serviceType, out var descriptor))
                return descriptor.GetInstance(this);

            throw new InvalidOperationException($"Service of type {serviceType} is not registered: {serviceType}");
        }

        /// <summary>
        /// 등록된 서비스들 간의 의존성을 주입합니다.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void BindEachOther()
        {
            foreach (var descriptor in _services.Values)
            {
                var instance = descriptor.GetInstance(this);
                if (instance == null) continue;

                var type = instance.GetType();
                while (type != null)
                {
                    var privateFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    foreach (var field in privateFields)
                    {
                        if (field.GetCustomAttribute<InjectAttribute>() == null) continue;
                        if (_services.ContainsKey(field.FieldType))
                            field.SetValue(instance, Resolve(field.FieldType));
                        else
                            throw new InvalidOperationException($"Service of type {field.FieldType} is not registered for injection into {type}.{field.Name}");
                    }
                    type = type.BaseType;
                }
            }
        }
    }
}