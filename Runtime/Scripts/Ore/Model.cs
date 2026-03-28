namespace Moonstone.Ore
{
    public abstract class Model
    {
        protected string id;

        public string Id => id;

        public Model(string id) => this.id = id ?? System.Guid.NewGuid().ToString();

        public static string NewId() => System.Guid.NewGuid().ToString();
    }
}