using System.Threading;
using System.Threading.Tasks;

namespace Moonstone.D3.Application
{
    public interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand
    {
        Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
