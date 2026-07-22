using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Moonstone.Core
{
    public static class LifecycleRunner
    {
        public static void InitializeHierarchy(params GameObject[] rootObjects)
        {
            foreach (var initializable in GetComponentsInHierarchy(rootObjects).OfType<Moonstone.IInitializable>())
                initializable.Initialize();
        }

        public static async Task InitializeHierarchyAsync(params GameObject[] rootObjects)
        {
            var initialized = new HashSet<object>();

            foreach (var component in GetComponentsInHierarchy(rootObjects))
            {
                if (component is Moonstone.IAsyncInitializable asyncInitializable)
                {
                    initialized.Add(asyncInitializable);
                    await asyncInitializable.InitializeAsync();
                    continue;
                }

                if (component is Moonstone.IInitializable initializable && initialized.Add(initializable))
                    initializable.Initialize();
            }
        }

        public static void DisposeHierarchy(params GameObject[] rootObjects)
        {
            var disposed = new HashSet<object>();
            foreach (var component in GetComponentsInHierarchy(rootObjects, parentFirst: false))
            {
                if (component is Moonstone.ILifecycleDisposable lifecycleDisposable && disposed.Add(lifecycleDisposable))
                    lifecycleDisposable.Dispose();

                if (component is System.IDisposable disposable && disposed.Add(disposable))
                    disposable.Dispose();
            }
        }

        private static IEnumerable<MonoBehaviour> GetComponentsInHierarchy(GameObject[] rootObjects, bool parentFirst = true)
        {
            var components = new List<MonoBehaviour>();
            var visitedTransforms = new HashSet<Transform>();

            foreach (var rootObject in rootObjects)
            {
                if (rootObject == null) continue;
                CollectComponents(rootObject.transform, components, visitedTransforms);
            }

            if (!parentFirst)
                components.Reverse();

            return components;
        }

        private static void CollectComponents(Transform transform, List<MonoBehaviour> components, HashSet<Transform> visitedTransforms)
        {
            if (transform == null || !visitedTransforms.Add(transform)) return;

            transform.GetComponents(components);

            for (int i = 0; i < transform.childCount; i++)
                CollectComponents(transform.GetChild(i), components, visitedTransforms);
        }
    }
}
