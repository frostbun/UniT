namespace UniT.UI.Item
{
    public interface IItemView : IView
    {
        protected internal object Model { set; }

        protected internal void OnShow();

        protected internal void OnHide();

        protected internal void OnDispose();
    }

    public interface IItemViewWithPresenter : IItemView, IViewWithPresenter
    {
    }
}