namespace Moonstone.Core.Service
{
    public interface IService
    {
        void Initialize();
        void Dispose();
        void SetUp();
        void TearDown();
    }
}