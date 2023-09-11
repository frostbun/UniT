namespace UniT.UI.Item
{
    using System.Threading;

    public interface IItemView : IView
    {
        public object Model { set; }

        public CancellationToken GetCancellationTokenOnHide();

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }
}