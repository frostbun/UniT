namespace UniT.UI.Item
{
    public interface IItemView : IView
    {
        public object Item { set; }

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }
}