namespace UniT.UI.Screen
{
    public interface IScreenView : IView
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

        public IScreenView PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        public void Stack(bool force = false);

        public void Float(bool force = false);

        public void Dock(bool force = false);

        public void Hide(bool removeFromStack = true, bool autoStack = true);

        public void Dispose(bool autoStack = true);
    }

    public interface IScreenViewWithPresenter : IScreenView, IViewWithPresenter
    {
    }
}