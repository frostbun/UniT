namespace UniT.UI
{
    using System;
    using UnityEngine;

    public abstract class BaseView : MonoBehaviour, IView
    {
        public IUIManager Manager { get; private set; }

        protected virtual void OnInitialize()
        {
        }

        #region Interface Implementation

        IUIManager IView.Manager { get => this.Manager; set => this.Manager = value; }

        void IView.OnInitialize() => this.OnInitialize();

        GameObject IView.GameObject => this.gameObject;

        Transform IView.Transform => this.transform;

        #endregion
    }

    public abstract class BaseView<TPresenter> : BaseView, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}