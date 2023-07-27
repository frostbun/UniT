namespace UniT.Signal
{
    using System;
    using UniT.Logging;

    public interface ISignalBus
    {
        public ILogger Logger { get; }

        public void Fire<T>(T signal);

        public void Subscribe<T>(Action<T> callback);

        public void Subscribe<T>(Action callback);

        public void Unsubscribe<T>(Action<T> callback);

        public void Unsubscribe<T>(Action callback);
    }
}