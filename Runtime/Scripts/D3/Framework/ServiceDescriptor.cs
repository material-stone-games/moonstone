using System;

namespace Moonstone.D3.Framework
{
    public class ServiceDescriptor
    {
        private object _singletonInstance;
        public Lifetime Lifetime { get; }
        public Func<IResolver, object> ImplementationFactory { get; }

        public ServiceDescriptor(Lifetime lifetime, Func<IResolver, object> implementationFactory)
        {
            Lifetime = lifetime;
            ImplementationFactory = implementationFactory;
        }

        public object GetInstance(IResolver resolver)
        {
            return Lifetime switch
            {
                Lifetime.Transient => ImplementationFactory(resolver),
                Lifetime.Singleton => _singletonInstance ??= ImplementationFactory(resolver),
                _ => throw new InvalidOperationException($"Unknown lifetime: {Lifetime}"),
            };
        }
    }
}