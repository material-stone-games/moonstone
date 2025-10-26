namespace Moonstone.Arc
{
    public class Container
    {
        private static Container _instance;
        public static Container Instance => _instance ??= new Container();

        private readonly Framework.ServiceResolver _serviceResolver;

        public Container() => _serviceResolver = new Framework.ServiceResolver();

        public static void Register<TImplementation>(params object[] constructorArgs) where TImplementation : class
            => Instance._serviceResolver.Register<TImplementation>(constructorArgs);
        public static void Register<TImplementation>(TImplementation instance)
            => Instance._serviceResolver.Register(instance);

        public static T Resolve<T>() => Instance._serviceResolver.Resolve<T>();
        public static object Resolve(System.Type serviceType) => Instance._serviceResolver.Resolve(serviceType);
        public static void BindEachOther()
            => Instance._serviceResolver.BindEachOther();

        public static void Dispose() => _instance = null;
    }
}