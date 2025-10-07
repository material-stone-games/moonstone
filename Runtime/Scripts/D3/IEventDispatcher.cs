using System.Threading.Tasks;

namespace Moonstone.D3
{
    public interface IEventDispatcher
    {
        Task Dispatch(Event @event);
        void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : Event;
        void RegisterHandler<TEvent>(Application.IApplicationService service) where TEvent : Event;
        void ClearHandlers();
    }
}