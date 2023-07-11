namespace UniT.UI
{
    using UnityEngine;

    public interface IView
    {
        protected internal GameObject GameObject { get; }

        protected internal Transform Transform { get; }

        protected internal IContract Contract { set; }

        protected internal IPresenter Presenter { set; }

        protected internal void Initialize();

        protected internal void Show();

        protected internal void Hide();

        protected internal void Dispose();
    }
}