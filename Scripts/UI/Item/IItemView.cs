namespace UniT.UI.Item
{
    public interface IItemView : IView
    {
        protected internal object Item { set; }
    }

    public interface IItemViewWithPresenter : IItemView, IViewWithPresenter
    {
    }
}