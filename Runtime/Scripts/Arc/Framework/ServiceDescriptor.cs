using System;

namespace Moonstone.Arc.Framework
{
    public class ServiceDescriptor
    {
        private object _singletonInstance;
        public Func<IResolver, object> ImplementationFactory { get; }

        public ServiceDescriptor(Func<IResolver, object> implementationFactory) => ImplementationFactory = implementationFactory;

        public object GetInstance(IResolver resolver) => _singletonInstance ??= ImplementationFactory(resolver);
    }
}