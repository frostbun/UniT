namespace UniT.Reactive
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public class ReactiveProperty<T>
    {
        private          T                  _value;
        private readonly HashSet<Action<T>> _callbacks = new();

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this._callbacks.SafeForEach(callback => callback(value));
            }
        }

        public ReactiveProperty(T value = default)
        {
            this._value = value;
        }

        public bool Subscribe(Action<T> callback, bool invokeImmediately = true)
        {
            if (!this._callbacks.Add(callback)) return false;
            if (invokeImmediately) callback(this._value);
            return true;
        }

        public bool Unsubscribe(Action<T> callback)
        {
            return this._callbacks.Remove(callback);
        }
    }
}