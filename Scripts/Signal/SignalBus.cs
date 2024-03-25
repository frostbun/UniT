namespace UniT.Signal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class SignalBus : ISignalBus
    {
        #region Public

        void ISignalBus.Fire<T>(T signal)
        {
            this.GetCallbacksWithSignal<T>()
                .Cast<Action<T>>()
                .SafeForEach(callback => callback(signal));
            this.GetCallbacksWithoutSignal<T>()
                .Cast<Action>()
                .SafeForEach(callback => callback());
        }

        bool ISignalBus.Subscribe<T>(Action<T> callback)
        {
            return this.GetCallbacksWithSignal<T>().Add(callback);
        }

        bool ISignalBus.Subscribe<T>(Action callback)
        {
            return this.GetCallbacksWithoutSignal<T>().Add(callback);
        }

        bool ISignalBus.Unsubscribe<T>(Action<T> callback)
        {
            return this.GetCallbacksWithSignal<T>().Remove(callback);
        }

        bool ISignalBus.Unsubscribe<T>(Action callback)
        {
            return this.GetCallbacksWithoutSignal<T>().Remove(callback);
        }

        #endregion

        #region Private

        private readonly Dictionary<Type, HashSet<object>> callbacksWithSignal    = new Dictionary<Type, HashSet<object>>();
        private readonly Dictionary<Type, HashSet<object>> callbacksWithoutSignal = new Dictionary<Type, HashSet<object>>();

        private HashSet<object> GetCallbacksWithSignal<T>()
        {
            return this.callbacksWithSignal.GetOrAdd(typeof(T));
        }

        private HashSet<object> GetCallbacksWithoutSignal<T>()
        {
            return this.callbacksWithoutSignal.GetOrAdd(typeof(T));
        }

        #endregion
    }
}