namespace UniT.UI
{
    using System;
    using UnityEngine;

    public abstract class View : MonoBehaviour, IView
    {
        IUIManager IView.Manager { get => this.Manager; set => this.Manager = value; }

        void IView.OnInitialize() => this.OnInitialize();

        public IUIManager Manager { get; private set; }

        protected virtual void OnInitialize()
        {
        }
    }

    public abstract class View<TPresenter> : View, IHasPresenter where TPresenter : IPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType { get; } = typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}