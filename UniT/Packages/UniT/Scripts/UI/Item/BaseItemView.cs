namespace UniT.UI.Item
{
    using System;

    public abstract class BaseItemView<TItem> : BaseView, IItemView
    {
        object IItemView.Item { set => this.Item = (TItem)value; }

        public TItem Item { get; private set; }
    }

    public abstract class BaseItemView<TItem, TPresenter> : BaseItemView<TItem>, IItemViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}