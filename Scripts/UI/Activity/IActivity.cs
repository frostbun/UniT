namespace UniT.UI.Activity
{
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

        protected internal void OnShow();

        protected internal void OnHide();

        protected internal void OnDispose();
    }

    public interface IActivityWithPresenter : IActivity, IViewWithPresenter
    {
    }
}