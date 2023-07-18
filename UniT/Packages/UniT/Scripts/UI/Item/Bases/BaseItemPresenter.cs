namespace UniT.UI.Item.Bases
{
    using UniT.UI.Item.Interfaces;

    public abstract class BaseItemPresenter<TView> : IItemPresenter where TView : IItemViewWithPresenter
    {
        IItemView IItemPresenter.View { set => this.View = (TView)value; }

        protected TView View { get; private set; }
    }
}