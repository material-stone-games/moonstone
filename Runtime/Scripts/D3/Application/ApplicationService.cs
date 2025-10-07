using System.Threading.Tasks;

namespace Moonstone.D3.Application
{
    public class HandlerAttribute : System.Attribute
    {
        public System.Type CommandType { get; }

        public HandlerAttribute(System.Type commandType)
        {
            CommandType = commandType;
        }
    }

    public abstract class ApplicationService : IApplicationService
    {
        private readonly IEventDispatcher _eventDispatcher;

        protected async Task DispatchDomainEvents(Domain.IAggregate aggregate)
        {
            var events = aggregate.GetUncommittedEvents();
            foreach (var domainEvent in events)
            {
                await _eventDispatcher.Dispatch(domainEvent);
            }
            aggregate.ClearEvents();
        }

        protected async Task Dispatch(Command command)
        {
            await _eventDispatcher.Dispatch(command);
        }
    }
}