namespace Moonstone.Arc.Events
{
    public interface IEventBus
    {
        System.IDisposable Subscribe<TEvent>(System.Action<TEvent> handler);
        System.IDisposable Subscribe<TEvent>(System.Func<TEvent, System.Threading.Tasks.Task> handler);
        System.Threading.Tasks.Task Publish<TEvent>(TEvent @event);
    }
}
