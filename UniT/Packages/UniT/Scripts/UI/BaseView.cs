namespace UniT.UI
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Utils;
    using UnityEngine;

    public abstract class BaseView<TPresenter> : MonoBehaviour, IView where TPresenter : IPresenter
    {
        GameObject IView.gameObject => this.gameObject;

        Transform IView.transform => this.transform;

        IPresenter IView.Presenter { set => this.Presenter = (TPresenter)value; }

        IViewManager.IViewInstance IView.Instance { set => this.Instance = value; }

        void IInitializable.Initialize()
        {
            this.GetComponentsInChildren<IInitializable>(true)
                .Skip(1)
                .ForEach(initializable => initializable.Initialize());
            this.Initialize();
        }

        void IDisposable.Dispose()
        {
            this.GetComponentsInChildren<IDisposable>(true)
                .Skip(1)
                .ForEach(disposable => disposable.Dispose());
            this.Dispose();
        }

        void IView.OnShow() => this.OnShow();

        void IView.OnHide() => this.OnHide();

        protected TPresenter Presenter { get; private set; }

        public IViewManager.IViewInstance Instance { get; private set; }

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