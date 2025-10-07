using System;

namespace Moonstone.D3.Presentation
{
    public interface IView : IInitializable, IDisposable
    {
        void BindViews();
        T GetView<T>() where T : IView;
        void Show();
        void Hide();
    }
}