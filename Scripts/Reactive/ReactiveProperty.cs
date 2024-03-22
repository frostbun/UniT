namespace UniT.Reactive
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public class ReactiveProperty<T>
    {
        private T value;

        private readonly HashSet<Action<T>> callbacks = new HashSet<Action<T>>();

        public ReactiveProperty(T value = default)
        {
            this.value = value;
        }

        public T Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.callbacks.SafeForEach(callback => callback(value));
            }
        }

        public bool Subscribe(Action<T> callback, bool invokeImmediately = true)
        {
            if (!this.callbacks.Add(callback)) return false;
            if (invokeImmediately) callback(this.value);
            return true;
        }

        public bool Unsubscribe(Action<T> callback)
        {
            return this.callbacks.Remove(callback);
        }
    }
}