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
        #region Constructor

        private readonly Dictionary<Type, HashSet<object>> _callbacksWithSignal;
        private readonly Dictionary<Type, HashSet<object>> _callbacksNoSignal;
        private readonly ILogger                           _logger;

        [Preserve]
        public SignalBus(ILogger logger = null)
        {
            this._callbacksWithSignal = new();
            this._callbacksNoSignal   = new();
            this._logger              = logger ?? ILogger.Default(this.GetType().Name);
            this._logger.Debug("Constructed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public void Fire<T>(T signal)
        {
            var count = this.GetCallbacksWithSignal<T>().Count + this.GetCallbacksNoSignal<T>().Count;
            if (count == 0)
            {
                this._logger.Warning($"No subscribers found for {typeof(T).Name}");
                return;
            }
            this.GetCallbacksWithSignal<T>()
                .Cast<Action<T>>()
                .SafeForEach(callback => callback(signal));
            this.GetCallbacksNoSignal<T>()
                .Cast<Action>()
                .SafeForEach(callback => callback());
            this._logger.Debug($"Fired {typeof(T).Name} to {count} subscribers");
        }

        public void Subscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Add(callback))
            {
                this._logger.Debug($"Subscribed {callback.Method.Name} to {typeof(T).Name}");
            }
            else
            {
                this._logger.Warning($"{callback.Method.Name} already subscribed to {typeof(T).Name}");
            }
        }

        public void Subscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Add(callback))
            {
                this._logger.Debug($"Subscribed {callback.Method.Name} to {typeof(T).Name}");
            }
            else
            {
                this._logger.Warning($"{callback.Method.Name} already subscribed to {typeof(T).Name}");
            }
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Remove(callback))
            {
                this._logger.Debug($"Unsubscribed {callback.Method.Name} from {typeof(T).Name}");
            }
            else
            {
                this._logger.Warning($"{callback.Method.Name} not subscribed to {typeof(T).Name}");
            }
        }

        public void Unsubscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Remove(callback))
            {
                this._logger.Debug($"Unsubscribed {callback.Method.Name} from {typeof(T).Name}");
            }
            else
            {
                this._logger.Warning($"{callback.Method.Name} not subscribed to {typeof(T).Name}");
            }
        }

        #endregion

        #region Private

        private HashSet<object> GetCallbacksWithSignal<T>()
        {
            return this._callbacksWithSignal.GetOrAdd(typeof(T), () => new HashSet<object>());
        }

        private HashSet<object> GetCallbacksNoSignal<T>()
        {
            return this._callbacksNoSignal.GetOrAdd(typeof(T), () => new HashSet<object>());
        }

        #endregion
    }
}