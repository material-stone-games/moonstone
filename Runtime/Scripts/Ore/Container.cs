using System;
using System.Collections.Generic;

namespace Moonstone.Ore
{
    public class Container : Local.Entity
    {
        private readonly Dictionary<Type, object> entities = new();
        private readonly HashSet<object> injectees = new();

        public void Register(params object[] bindees)
        {
            foreach (var bindee in bindees)
            {
                if (bindee == null)
                    throw new ArgumentNullException(nameof(bindee));

                var type = bindee.GetType();

                if (entities.ContainsKey(type))
                    throw new ArgumentException($"Entity already registered for type {type.FullName}");

                entities.Add(type, bindee);
                injectees.Add(bindee);
            }
        }

        public bool Resolve<T>(out T entity)
        {
            if (TryResolveDependency(typeof(T), out var dependency) && dependency is T resolved)
            {
                entity = resolved;
                return true;
            }

            entity = default!;
            return false;
        }

        public void AddInjectee(params object[] objects)
        {
            foreach (var obj in objects)
            {
                injectees.Add(obj);
            }
        }

        public void Bind()
        {
            foreach (var injectee in injectees)
                InjectIntoObject(injectee);
        }

        public void Inject(object injectee)
        {
            InjectIntoObject(injectee);
        }

        private void InjectIntoObject(object target)
        {
            if (target == null)
                return;

            var type = target.GetType();
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

            foreach (var field in type.GetFields(flags))
            {
                if (field.IsInitOnly)
                    continue;

                if (field.GetValue(target) != null)
                    continue;

                if (TryResolveDependency(field.FieldType, out var dependency))
                {
                    field.SetValue(target, dependency);
                }
            }

            foreach (var property in type.GetProperties(flags))
            {
                if (!property.CanWrite || property.GetIndexParameters().Length > 0)
                    continue;

                if (property.GetValue(target) != null)
                    continue;

                if (TryResolveDependency(property.PropertyType, out var dependency))
                {
                    property.SetValue(target, dependency);
                }
            }
        }

        private bool TryResolveDependency(Type dependencyType, out object dependency)
        {
            if (entities.TryGetValue(dependencyType, out dependency))
                return true;

            foreach (var kv in entities)
            {
                if (dependencyType.IsAssignableFrom(kv.Key))
                {
                    dependency = kv.Value;
                    return true;
                }
            }

            dependency = null;
            return false;
        }
    }
}