namespace UniT.UI
{
    using UnityEngine;

    public interface IView
    {
        protected internal GameObject gameObject { get; }

        protected internal Transform transform { get; }

        protected internal IPresenter Presenter { set; }

        protected internal IViewManager.IViewInstance Instance { set; }

        protected internal void Initialize();

        protected internal void Dispose();

        protected internal void OnShow();

        protected internal void OnHide();
    }
}