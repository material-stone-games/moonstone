namespace Moonstone.Arc
{
    public class Model
    {
        private readonly string _id;

        public string Id => _id;

        public Model() => _id = System.Guid.NewGuid().ToString();

        public Model(string id) => _id = id;

        public static string NewId() => System.Guid.NewGuid().ToString();
    }
}