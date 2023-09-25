namespace UniT.UI.Item
{
    using System.Threading;

    public interface IItemView : IView
    {
        public object Item { set; }

        public CancellationToken GetCancellationTokenOnHide();

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }
}