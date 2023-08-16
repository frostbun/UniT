namespace UniT.UI.Activity
{
    using Cysharp.Threading.Tasks;

    public interface IActivity : IView
    {
        public enum Status
        {
            Stacking,
            Floating,
            Docked,
            Hidden,
            Disposed,
        }

        public Status CurrentStatus { get; protected internal set; }

        public IActivity PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        public UniTask<T> WaitForResult<T>();

        protected internal void OnShow();

        protected internal void OnHide();

        protected internal void OnDispose();
    }

    public interface IActivityWithPresenter : IActivity, IViewWithPresenter
    {
    }
}