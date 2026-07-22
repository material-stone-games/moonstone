using System.Threading;
using System.Threading.Tasks;

namespace Moonstone.D3.Application
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken = default);
    }
}
