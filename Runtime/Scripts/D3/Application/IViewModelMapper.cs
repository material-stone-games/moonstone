namespace Moonstone.D3.Application
{
    public interface IViewModelMapper<TEntity, TViewModel>
    {
        TViewModel ToViewModel(TEntity entity);
    }

    public interface IViewModelMapper<TEntity, TViewModel, TParam>
    {
        TViewModel ToViewModel(TEntity entity, TParam param);
    }
}