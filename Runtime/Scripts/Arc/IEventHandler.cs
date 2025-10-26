using System.Threading.Tasks;

namespace Moonstone.Arc
{
    public interface IEventHandler
    {
        Task Handle<TEnum>(TEnum @enum, params object[] arguments) where TEnum : System.Enum;
    }
}