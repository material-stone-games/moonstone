using System.Threading;
using System.Threading.Tasks;

namespace Moonstone.D3.Application
{
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken = default);
    }
}
