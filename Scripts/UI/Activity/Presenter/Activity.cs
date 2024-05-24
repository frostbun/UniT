#nullable enable
namespace UniT.UI.Activity.Presenter
{
    using System;
    using UniT.UI.Presenter;

    public abstract class Activity<TPresenter> : Activity, IHasPresenter where TPresenter : IActivityPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; } = default!;

        protected sealed override void OnInitialize() => this.Presenter.OnInitialize();

        protected sealed override void OnShow() => this.Presenter.OnShow();

        protected sealed override void OnHide() => this.Presenter.OnHide();
    }

    public abstract class Activity<TParams, TPresenter> : UI.Activity.Activity<TParams>, IHasPresenter where TPresenter : IActivityPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; } = default!;

        protected sealed override void OnInitialize() => this.Presenter.OnInitialize();

        protected sealed override void OnShow() => this.Presenter.OnShow();

        protected sealed override void OnHide() => this.Presenter.OnHide();
    }
}