namespace UniT.UI.Item
{
    public interface IItemView : IView
    {
        public object Model { set; }

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }

    public interface IItemViewWithPresenter : IItemView, IViewWithPresenter
    {
    }
}