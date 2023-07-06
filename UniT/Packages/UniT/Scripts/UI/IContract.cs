namespace UniT.UI
{
    using System;

    public interface IContract : IDisposable
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

        public void Stack();

        public void Float();

        public void Dock();

        public void Hide();
    }
}