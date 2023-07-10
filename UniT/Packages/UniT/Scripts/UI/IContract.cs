namespace UniT.UI
{
    public interface IContract
    {
        public enum Status
        {
            Disposed,
            Hidden,
            Stacking,
            Floating,
            Docked,
        }

        public Status CurrentStatus { get; }

        public IContract PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        public void Stack(bool force = false);

        public void Float(bool force = false);

        public void Dock(bool force = false);

        public void Hide(bool autoStack = true);

        public void Dispose(bool autoStack = true);
    }
}