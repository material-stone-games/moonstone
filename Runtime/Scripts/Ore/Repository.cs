using System.Collections.Generic;

namespace Moonstone.Ore
{
    public abstract class Repository<TModel> where TModel : Model
    {
        protected readonly Dictionary<string, TModel> aggregates = new();

        public void Save(TModel model)
        {
            aggregates[model.Id] = model;
        }

        public void Delete(string id)
        {
            aggregates.Remove(id);
        }

        public TModel FindById(string id)
        {
            aggregates.TryGetValue(id, out var model);
            return model;
        }

        public IReadOnlyCollection<TModel> FindAll()
        {
            return aggregates.Values;
        }
    }
}
