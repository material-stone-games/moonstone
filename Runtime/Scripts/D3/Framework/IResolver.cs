using System;

namespace Moonstone.D3.Framework
{
    public interface IResolver
    {
        T Resolve<T>();
        object Resolve(Type serviceType);
    }
}