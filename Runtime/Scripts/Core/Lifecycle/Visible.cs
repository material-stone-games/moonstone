namespace Moonstone.Core
{
    public abstract class Visible : LifecycleBehaviour, Moonstone.IVisible
    {
        public void SetVisibility(bool visible)
            => gameObject.SetActive(visible);

        public void Show()
            => SetVisibility(true);

        public void Hide()
            => SetVisibility(false);
    }
}
