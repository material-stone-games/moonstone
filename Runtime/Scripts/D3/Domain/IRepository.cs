using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moonstone.D3.Domain
{
    public interface IRepository<TAggregate, TId>
        where TAggregate : IAggregateRoot<TId>
    {
        Task Save(TAggregate aggregate, CancellationToken cancellationToken = default);
        Task Delete(TId id, CancellationToken cancellationToken = default);
        Task<TAggregate> FindById(TId id, CancellationToken cancellationToken = default);
        Task<bool> Exists(TId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<TAggregate>> FindAll(CancellationToken cancellationToken = default);
    }
}
