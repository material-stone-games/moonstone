using System.Threading;
using System.Threading.Tasks;
using Moonstone.D3.Domain;

namespace Moonstone.D3.Application
{
    public interface IDomainEventPublisher
    {
        Task Publish(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
