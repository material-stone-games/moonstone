namespace Moonstone.Ore
{
    public abstract class Model
    {
        protected string id;

        public string Id => id;

        public Model(string id) => this.id = id;

        public Model() : this(System.Guid.NewGuid().ToString()) { }
    }
}