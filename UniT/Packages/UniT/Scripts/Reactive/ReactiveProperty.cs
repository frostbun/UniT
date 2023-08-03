namespace UniT.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ReactiveProperty<T>
    {
        private class Subscriber : IDisposable
        {
            public readonly  Action<T>           _callback;
            private readonly ReactiveProperty<T> _property;
            private          bool                _isDisposed;

            public Subscriber(Action<T> callback, ReactiveProperty<T> property)
            {
                this._callback   = callback;
                this._property   = property;
                this._isDisposed = false;
            }

            public void Dispose()
            {
                if (this._isDisposed) throw new ObjectDisposedException(nameof(Subscriber));
                this._property._subscribers.Remove(this);
                this._isDisposed = true;
            }
        }

        private          T                _value;
        private readonly List<Subscriber> _subscribers = new(); // Allowing multiple subscriptions

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this._subscribers.ToList().ForEach(subscriber => subscriber._callback(value));
            }
        }

        public ReactiveProperty(T value = default)
        {
            this._value = value;
        }

        public IDisposable Subscribe(Action<T> callback, bool invokeImmediately = true)
        {
            var subscriber = new Subscriber(callback, this);
            this._subscribers.Add(subscriber);
            if (invokeImmediately) callback(this._value);
            return subscriber;
        }

        public void Unsubscribe(Action<T> callback)
        {
            this._subscribers.Find(subscriber => subscriber._callback == callback)?.Dispose();
        }
    }
}