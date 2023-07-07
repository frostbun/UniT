namespace UniT.UI
{
    using UnityEngine;

    public abstract class BaseView<TPresenter> : MonoBehaviour, IView where TPresenter : IPresenter
    {
        GameObject IView.GameObject => this.gameObject;

        Transform IView.Transform => this.transform;

        IContract IView.Contract { set => this.Contract = value; }

        IPresenter IView.Presenter { set => this.Presenter = (TPresenter)value; }

        void IView.Initialize() => this.Initialize();

        void IView.OnShow() => this.OnShow();

        void IView.OnHide() => this.OnHide();

        void IView.Dispose() => this.Dispose();

        protected IContract Contract { get; private set; }

        protected TPresenter Presenter { get; private set; }

        protected virtual void Initialize()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void Dispose()
        {
        }
    }
}