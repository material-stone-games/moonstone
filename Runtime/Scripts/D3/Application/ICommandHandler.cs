using System.Threading;
using System.Threading.Tasks;

namespace Moonstone.D3.Application
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command, CancellationToken cancellationToken = default);
    }
}
