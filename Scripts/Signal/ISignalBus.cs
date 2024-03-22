namespace UniT.Signal
{
    using System;

    public interface ISignalBus
    {
        public void Fire<T>(T signal);

        public bool Subscribe<T>(Action<T> callback);

        public bool Subscribe<T>(Action callback);

        public bool Unsubscribe<T>(Action<T> callback);

        public bool Unsubscribe<T>(Action callback);
    }
}