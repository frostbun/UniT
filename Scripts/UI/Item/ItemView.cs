namespace UniT.UI.Item
{
    using System;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class ItemView<TItem> : View, IItemView
    {
        object IItemView.Item { set => this.Item = (TItem)value; }

        void IItemView.OnShow() => this.OnShow();

        void IItemView.OnHide()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnHide();
        }

        void IItemView.OnDispose() => this.OnDispose();

        public TItem Item { get; private set; }

        #if UNIT_UNITASK
        private CancellationTokenSource hideCts;

        protected CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new()).Token;
        }
        #endif

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

    public abstract class ItemView<TItem, TPresenter> : ItemView<TItem>, IHasPresenter where TPresenter : IPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}