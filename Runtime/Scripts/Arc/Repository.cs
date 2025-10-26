using System.Collections.Generic;
using Moonstone.Arc.Framework;

namespace Moonstone.Arc
{
    public class Repository<TModel> where TModel : Model
    {
        protected readonly Dictionary<string, TModel> _storage = new();

        public TModel GetById(string id)
        {
            if (_storage.TryGetValue(id, out var model))
                return model;
            else
                throw new KeyNotFoundException($"Model with ID {id} not found.");
        }

        public void Save(TModel model)
        {
            _storage[model.Id] = model; // TODO: 이미 있을 때 어떻게 처리할지 생각
        }

        public void Delete(string id)
        {
            _storage.Remove(id);
        }
    }
}