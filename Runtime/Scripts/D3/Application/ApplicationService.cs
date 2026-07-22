using System;
using System.Threading;
using System.Threading.Tasks;
using Moonstone.D3.Domain;

namespace Moonstone.D3.Application
{
    public abstract class ApplicationService : IApplicationService
    {
        private readonly IDomainEventPublisher _domainEventPublisher;

        protected ApplicationService(IDomainEventPublisher domainEventPublisher)
        {
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        protected async Task PublishDomainEvents<TId>(IAggregateRoot<TId> aggregate, CancellationToken cancellationToken = default)
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var events = aggregate.GetUncommittedEvents();
            foreach (var domainEvent in events)
                await _domainEventPublisher.Publish(domainEvent, cancellationToken);

            aggregate.ClearEvents();
        }

        protected async Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            await _domainEventPublisher.Publish(domainEvent, cancellationToken);
        }
    }
}
