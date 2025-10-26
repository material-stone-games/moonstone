using System;

namespace Moonstone.Arc.Framework
{
    public interface IResolver
    {
        T Resolve<T>();
        object Resolve(Type serviceType);
    }
}