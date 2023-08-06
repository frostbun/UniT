namespace UniT.UI.Screen
{
    public interface IScreen : IView
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

        public IScreen PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        protected internal void OnShow();

        protected internal void OnHide();

        protected internal void OnDispose();
    }

    public interface IScreenWithPresenter : IScreen, IViewWithPresenter
    {
    }
}