namespace UniT.UI
{
    using System;
    using UnityEngine;

    public abstract class BaseView : MonoBehaviour, IView
    {
        IUIManager IView.Manager { get => this.Manager; set => this.Manager = value; }

        void IView.OnInitialize() => this.OnInitialize();

        void IView.OnShow() => this.OnShow();

        void IView.OnHide() => this.OnHide();

        void IView.OnDispose() => this.OnDispose();

        public IUIManager Manager { get; private set; }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnDispose()
        {
        }
    }

    public abstract class BaseView<TPresenter> : BaseView, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}