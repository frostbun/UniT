namespace UniT.UI
{
    using System;
    using UniT.Utilities;
    using UnityEngine;

    public abstract class BaseView<TPresenter> : MonoBehaviour, IView where TPresenter : IPresenter
    {
        GameObject IView.GameObject => this.gameObject;

        Transform IView.Transform => this.transform;

        IContract IView.Contract { set => this.Contract = value; }

        IPresenter IView.Presenter { set => this.Presenter = (TPresenter)value; }

        void IInitializable.Initialize() => this.Initialize();

        void IDisposable.Dispose() => this.Dispose();

        void IView.OnShow() => this.OnShow();

        void IView.OnHide() => this.OnHide();

        protected IContract Contract { get; private set; }

        protected TPresenter Presenter { get; private set; }

        protected virtual void Initialize()
        {
        }

        protected virtual void Dispose()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
    }
}