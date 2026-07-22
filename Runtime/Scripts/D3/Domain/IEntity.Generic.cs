namespace Moonstone.D3.Domain
{
    public interface IEntity<out TId>
    {
        TId Id { get; }
    }
}
