using System;

namespace Moonstone.Arc.DependencyInjection
{
    public interface IResolver
    {
        T Resolve<T>();
        object Resolve(Type serviceType);
        bool TryResolve<T>(out T service);
        bool TryResolve(Type serviceType, out object service);
    }
}
