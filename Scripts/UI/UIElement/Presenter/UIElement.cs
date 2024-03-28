﻿namespace UniT.UI.UIElement.Presenter
{
    using System;
    using UniT.UI.Presenter;

    public abstract class UIElement<TPresenter> : UIElement, IHasPresenter where TPresenter : IUIElementPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }

        protected sealed override void OnInitialize() => this.Presenter.OnInitialize();

        protected sealed override void OnShow() => this.Presenter.OnShow();

        protected sealed override void OnHide() => this.Presenter.OnHide();

        protected sealed override void OnDispose() => this.Presenter.OnDispose();
    }

    public abstract class UIElement<TParams, TPresenter> : UI.UIElement.UIElement<TParams>, IHasPresenter where TPresenter : IUIElementPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }

        protected sealed override void OnInitialize() => this.Presenter.OnInitialize();

        protected sealed override void OnShow() => this.Presenter.OnShow();

        protected sealed override void OnHide() => this.Presenter.OnHide();

        protected sealed override void OnDispose() => this.Presenter.OnDispose();
    }
}