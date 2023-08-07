namespace UniT.Signal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public class SignalBus : ISignalBus
    {
        public LogConfig LogConfig => this._logger.Config;

        private readonly Dictionary<Type, HashSet<object>> _callbacksWithSignal = new();
        private readonly Dictionary<Type, HashSet<object>> _callbacksNoSignal   = new();
        private readonly ILogger                           _logger;

        [Preserve]
        public SignalBus(ILogger logger = null)
        {
            this._logger = logger ?? ILogger.Default(this.GetType().Name);
        }

        public void Fire<T>(T signal)
        {
            var count = this.GetCallbacksWithSignal<T>().Count + this.GetCallbacksNoSignal<T>().Count;
            if (count == 0)
            {
                this._logger.Warning($"No subscribers for signal {typeof(T)}");
                return;
            }
            this.GetCallbacksWithSignal<T>()
                .Cast<Action<T>>()
                .ForEach(callback => callback(signal));
            this.GetCallbacksNoSignal<T>()
                .Cast<Action>()
                .ForEach(callback => callback());
            this._logger.Debug($"Fired signal {typeof(T)} to {count} subscribers");
        }

        public void Subscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Add(callback)) return;
            this._logger.Warning($"Callback {callback} already subscribed to signal {typeof(T)}");
        }

        public void Subscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Add(callback)) return;
            this._logger.Warning($"Callback {callback} already subscribed to signal {typeof(T)}");
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Remove(callback)) return;
            this._logger.Warning($"Callback {callback} not subscribed to signal {typeof(T)}");
        }

        public void Unsubscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Remove(callback)) return;
            this._logger.Warning($"Callback {callback} not subscribed to signal {typeof(T)}");
        }

        private HashSet<object> GetCallbacksWithSignal<T>()
        {
            return this._callbacksWithSignal.GetOrAdd(typeof(T), () => new HashSet<object>());
        }

        private HashSet<object> GetCallbacksNoSignal<T>()
        {
            return this._callbacksNoSignal.GetOrAdd(typeof(T), () => new HashSet<object>());
        }
    }
}