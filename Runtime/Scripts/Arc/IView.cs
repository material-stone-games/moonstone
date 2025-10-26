using System;

namespace Moonstone.Arc
{
    public interface IView : IInitializable, IDisposable
    {
        void BindViews();
        T GetView<T>() where T : IView;
        void Show();
        void Hide();
    }
}