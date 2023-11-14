namespace UniT.Signal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine.Scripting;

    public sealed class SignalBus : ISignalBus
    {
        #region Constructor

        private readonly Dictionary<Type, HashSet<object>> callbacksWithSignal;
        private readonly Dictionary<Type, HashSet<object>> callbacksNoSignal;
        private readonly ILogger                           logger;

        [Preserve]
        public SignalBus(ILogger logger = null)
        {
            this.callbacksWithSignal = new();
            this.callbacksNoSignal   = new();
            this.logger              = logger ?? ILogger.Default(this.GetType().Name);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Finalizer

        ~SignalBus()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        public void Dispose()
        {
            this.callbacksWithSignal.Clear();
            this.callbacksNoSignal.Clear();
            this.logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public void Fire<T>(T signal)
        {
            var count = this.GetCallbacksWithSignal<T>().Count + this.GetCallbacksNoSignal<T>().Count;
            if (count == 0)
            {
                this.logger.Warning($"No subscribers found for {typeof(T).Name}");
                return;
            }
            this.GetCallbacksWithSignal<T>()
                .Cast<Action<T>>()
                .SafeForEach(callback => callback(signal));
            this.GetCallbacksNoSignal<T>()
                .Cast<Action>()
                .SafeForEach(callback => callback());
            this.logger.Debug($"Fired {typeof(T).Name} to {count} subscribers");
        }

        public void Subscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Add(callback))
            {
                this.logger.Debug($"Subscribed {callback.Method.Name} to {typeof(T).Name}");
            }
            else
            {
                this.logger.Warning($"{callback.Method.Name} already subscribed to {typeof(T).Name}");
            }
        }

        public void Subscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Add(callback))
            {
                this.logger.Debug($"Subscribed {callback.Method.Name} to {typeof(T).Name}");
            }
            else
            {
                this.logger.Warning($"{callback.Method.Name} already subscribed to {typeof(T).Name}");
            }
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Remove(callback))
            {
                this.logger.Debug($"Unsubscribed {callback.Method.Name} from {typeof(T).Name}");
            }
            else
            {
                this.logger.Warning($"{callback.Method.Name} not subscribed to {typeof(T).Name}");
            }
        }

        public void Unsubscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Remove(callback))
            {
                this.logger.Debug($"Unsubscribed {callback.Method.Name} from {typeof(T).Name}");
            }
            else
            {
                this.logger.Warning($"{callback.Method.Name} not subscribed to {typeof(T).Name}");
            }
        }

        #endregion

        #region Private

        private HashSet<object> GetCallbacksWithSignal<T>()
        {
            return this.callbacksWithSignal.GetOrAdd(typeof(T), () => new());
        }

        private HashSet<object> GetCallbacksNoSignal<T>()
        {
            return this.callbacksNoSignal.GetOrAdd(typeof(T), () => new());
        }

        #endregion
    }
}