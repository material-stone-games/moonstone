using UnityEngine;

namespace Moonstone.Core
{
    public class View : Visible
    {
        protected RectTransform rectTransform;

        protected virtual void Awake()
        {
            if (!IsInitialized)
                Initialize();
        }

        protected virtual void OnDestroy()
        {
            if (!IsDisposed)
                Dispose();
        }

        protected override void OnInitialize()
            => TryGetComponent(out rectTransform);

        protected override void OnDispose()
            => rectTransform = null;
    }
}
