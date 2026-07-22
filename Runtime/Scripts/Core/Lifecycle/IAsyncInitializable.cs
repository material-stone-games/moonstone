using System.Threading.Tasks;

namespace Moonstone
{
    public interface IAsyncInitializable
    {
        Task InitializeAsync();
    }
}
