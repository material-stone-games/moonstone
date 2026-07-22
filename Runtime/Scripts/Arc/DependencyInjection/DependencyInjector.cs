using System;
using System.Reflection;

namespace Moonstone.Arc.DependencyInjection
{
    internal static class DependencyInjector
    {
        public static void Inject(object target, IResolver resolver)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            var type = target.GetType();
            while (type != null)
            {
                InjectFields(target, type, resolver);
                InjectProperties(target, type, resolver);
                type = type.BaseType;
            }
        }

        private static void InjectFields(object target, Type type, IResolver resolver)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() == null) continue;

                if (field.IsInitOnly)
                    throw new InvalidOperationException($"Cannot inject readonly field {type.FullName}.{field.Name}.");

                field.SetValue(target, resolver.Resolve(field.FieldType));
            }
        }

        private static void InjectProperties(object target, Type type, IResolver resolver)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<InjectAttribute>() == null) continue;

                if (property.GetIndexParameters().Length > 0)
                    throw new InvalidOperationException($"Cannot inject indexed property {type.FullName}.{property.Name}.");

                if (!property.CanWrite)
                    throw new InvalidOperationException($"Cannot inject read-only property {type.FullName}.{property.Name}.");

                property.SetValue(target, resolver.Resolve(property.PropertyType));
            }
        }
    }
}
