namespace UniT.Utils
{
    using System;
    using System.Collections.Generic;

    public class ReactiveProperty<T>
    {
        private class Subscriber : IDisposable
        {
            public readonly  Action<T>           callback;
            private readonly ReactiveProperty<T> property;
            private          bool                isDisposed;

            public Subscriber(Action<T> callback, ReactiveProperty<T> property)
            {
                this.callback   = callback;
                this.property   = property;
                this.isDisposed = false;
            }

            public void Dispose()
            {
                if (this.isDisposed) throw new ObjectDisposedException(nameof(Subscriber));
                this.property.subscribers.Remove(this);
                this.isDisposed = true;
            }
        }

        private          T                value;
        private readonly List<Subscriber> subscribers = new(); // Allowing multiple subscriptions

        public T Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.subscribers.ForEach(subscriber => subscriber.callback(value));
            }
        }

        public ReactiveProperty(T value)
        {
            this.value = value;
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            var subscriber = new Subscriber(callback, this);
            this.subscribers.Add(subscriber);
            return subscriber;
        }

        public void Unsubscribe(Action<T> callback)
        {
            this.subscribers.Find(subscriber => subscriber.callback == callback)?.Dispose();
        }
    }
}