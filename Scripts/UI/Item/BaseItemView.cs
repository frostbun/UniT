namespace UniT.UI.Item
{
    using System;

    public abstract class BaseItemView<TItem> : BaseView, IItemView
    {
        public TItem Item { get; private set; }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnDispose()
        {
        }

        #region Interface Implementation

        object IItemView.Item { set => this.Item = (TItem)value; }

        void IItemView.OnShow() => this.OnShow();

        void IItemView.OnHide() => this.OnHide();

        void IItemView.OnDispose() => this.OnDispose();

        #endregion
    }

    public abstract class BaseItemView<TItem, TPresenter> : BaseItemView<TItem>, IItemViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}