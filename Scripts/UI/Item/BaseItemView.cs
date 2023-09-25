namespace UniT.UI.Item
{
    using System;
    using System.Threading;

    public abstract class BaseItemView<TItem> : BaseView, IItemView
    {
        object IItemView.Item { set => this.Item = (TItem)value; }

        void IItemView.OnShow() => this.OnShow();

        void IItemView.OnHide()
        {
            this._hideCts?.Cancel();
            this._hideCts?.Dispose();
            this._hideCts = null;
            this.OnHide();
        }

        void IItemView.OnDispose() => this.OnDispose();

        public TItem Item { get; private set; }

        private CancellationTokenSource _hideCts;

        public CancellationToken GetCancellationTokenOnHide()
        {
            return (this._hideCts ??= new()).Token;
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

    public abstract class BaseItemView<TModel, TPresenter> : BaseItemView<TModel>, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}