using UnityEngine;

namespace Moonstone.View.UI
{
    public class View : MonoBehaviour, IView
    {
        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);
    }
}