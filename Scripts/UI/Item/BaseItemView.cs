namespace UniT.UI.Item
{
    using System;

    public abstract class BaseItemView<TModel> : BaseView, IItemView
    {
        object IItemView.Model { set => this.Model = (TModel)value; }

        void IItemView.OnShow() => this.OnShow();

        void IItemView.OnHide() => this.OnHide();

        void IItemView.OnDispose() => this.OnDispose();

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
    }

    public abstract class BaseItemView<TModel, TPresenter> : BaseItemView<TModel>, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}