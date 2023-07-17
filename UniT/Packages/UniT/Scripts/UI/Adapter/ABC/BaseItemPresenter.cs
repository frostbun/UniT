namespace UniT.UI.Adapter.ABC
{
    using UniT.UI.Adapter.Interfaces;

    public abstract class BaseItemPresenter<TItem, TView> : IItemPresenter<TItem> where TView : IItemView<TItem>
    {
        IItemContract<TItem> IItemPresenter<TItem>.Contract { set => this.Contract = value; }

        IItemView<TItem> IItemPresenter<TItem>.View { set => this.View = (TView)value; }

        protected IItemContract<TItem> Contract { get; private set; }

        protected IItemView<TItem> View { get; private set; }
    }
}