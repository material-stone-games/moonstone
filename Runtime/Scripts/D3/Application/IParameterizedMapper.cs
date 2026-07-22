namespace Moonstone.D3.Application
{
    public interface IParameterizedMapper<in TSource, in TParameter, out TDestination>
    {
        TDestination Map(TSource source, TParameter parameter);
    }
}
