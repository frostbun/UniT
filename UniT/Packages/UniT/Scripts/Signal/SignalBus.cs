namespace UniT.Signal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.Logging;

    public class SignalBus : ISignalBus
    {
        public ILogger Logger { get; }

        private readonly Dictionary<Type, HashSet<object>> callbacksWithSignal = new();
        private readonly Dictionary<Type, HashSet<object>> callbacksNoSignal   = new();

        public SignalBus(ILogger logger = null)
        {
            this.Logger = logger ?? ILogger.Default(this.GetType().Name);
        }

        public void Fire<T>(T signal)
        {
            var count = this.GetCallbacksWithSignal<T>().Count + this.GetCallbacksNoSignal<T>().Count;
            if (count == 0)
            {
                this.Logger.Warning($"No subscribers for signal {typeof(T)}");
                return;
            }
            this.GetCallbacksWithSignal<T>()
                .Cast<Action<T>>()
                .ForEach(callback => callback(signal));
            this.GetCallbacksNoSignal<T>()
                .Cast<Action>()
                .ForEach(callback => callback());
            this.Logger.Debug($"Fired signal {typeof(T)} to {count} subscribers");
        }

        public void Subscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Add(callback)) return;
            this.Logger.Warning($"Callback {callback} already subscribed to signal {typeof(T)}");
        }

        public void Subscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Add(callback)) return;
            this.Logger.Warning($"Callback {callback} already subscribed to signal {typeof(T)}");
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            if (this.GetCallbacksWithSignal<T>().Remove(callback)) return;
            this.Logger.Warning($"Callback {callback} not subscribed to signal {typeof(T)}");
        }

        public void Unsubscribe<T>(Action callback)
        {
            if (this.GetCallbacksNoSignal<T>().Remove(callback)) return;
            this.Logger.Warning($"Callback {callback} not subscribed to signal {typeof(T)}");
        }

        private HashSet<object> GetCallbacksWithSignal<T>()
        {
            return this.callbacksWithSignal.GetOrAdd(typeof(T), () => new());
        }

        private HashSet<object> GetCallbacksNoSignal<T>()
        {
            return this.callbacksNoSignal.GetOrAdd(typeof(T), () => new());
        }
    }
}