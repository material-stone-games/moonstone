namespace Moonstone.Arc
{
    public sealed class Container
    {
        private static Container _instance;
        public static Container Instance => _instance ??= new Container();

        private readonly DependencyInjection.ServiceResolver _serviceResolver;

        private Container()
        {
            _serviceResolver = new DependencyInjection.ServiceResolver();
            _serviceResolver.Register<DependencyInjection.IResolver>(_serviceResolver);
        }

        public static void Register<TService>(TService instance) where TService : class
            => Instance._serviceResolver.Register(instance);
        public static void Register(System.Type serviceType, object instance)
            => Instance._serviceResolver.Register(serviceType, instance);

        public static T Resolve<T>() => Instance._serviceResolver.Resolve<T>();
        public static object Resolve(System.Type serviceType) => Instance._serviceResolver.Resolve(serviceType);
        public static bool TryResolve<T>(out T service) => Instance._serviceResolver.TryResolve(out service);
        public static bool TryResolve(System.Type serviceType, out object service) => Instance._serviceResolver.TryResolve(serviceType, out service);
        public static void BindEachOther()
            => Instance._serviceResolver.BindEachOther();
        internal static DependencyInjection.IResolver Resolver => Instance._serviceResolver;

        public static void Dispose()
        {
            _instance?._serviceResolver.Dispose();
            _instance = null;
        }
    }
}
