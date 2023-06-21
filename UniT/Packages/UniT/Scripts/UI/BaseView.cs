namespace UniT.UI
{
    using UnityEngine;

    public abstract class BaseView<TPresenter> : MonoBehaviour, IView where TPresenter : IPresenter
    {
        GameObject IView.gameObject => this.gameObject;

        Transform IView.transform => this.transform;

        IPresenter IView.Presenter { set => this.Presenter = (TPresenter)value; }

        IViewManager.IViewInstance IView.Instance { set => this.Instance = value; }

        void IView.OnShow() => this.OnShow();

        void IView.OnHide() => this.OnHide();

        protected TPresenter Presenter { get; private set; }

        public IViewManager.IViewInstance Instance { get; private set; }

        protected abstract void OnShow();

        protected abstract void OnHide();
    }
}