namespace UniT.UI.Item
{
    using System;

    public abstract class BaseItemView<TModel> : BaseView, IItemView
    {
        public TModel Model { get; private set; }

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

        object IItemView.Model { set => this.Model = (TModel)value; }

        void IItemView.OnShow() => this.OnShow();

        void IItemView.OnHide() => this.OnHide();

        void IItemView.OnDispose() => this.OnDispose();

        #endregion
    }

    public abstract class BaseItemView<TModel, TPresenter> : BaseItemView<TModel>, IItemViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}