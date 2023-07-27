namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;

    public static class SignalBus
    {
        private static readonly Dictionary<Type, HashSet<object>> CallbacksWithSignal = new();
        private static readonly Dictionary<Type, HashSet<object>> CallbacksNoSignal   = new();

        public static void Fire<T>(T signal)
        {
            GetCallbacksWithSignal<T>()
                .Cast<Action<T>>()
                .ForEach(callback => callback(signal));
            GetCallbacksNoSignal<T>()
                .Cast<Action>()
                .ForEach(callback => callback());
        }

        public static void Subscribe<T>(Action<T> callback)
        {
            GetCallbacksWithSignal<T>().Add(callback);
        }

        public static void Subscribe<T>(Action callback)
        {
            GetCallbacksNoSignal<T>().Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            GetCallbacksWithSignal<T>().Remove(callback);
        }

        public static void Unsubscribe<T>(Action callback)
        {
            GetCallbacksNoSignal<T>().Remove(callback);
        }

        private static HashSet<object> GetCallbacksWithSignal<T>()
        {
            return CallbacksWithSignal.GetOrAdd(typeof(T), () => new());
        }

        private static HashSet<object> GetCallbacksNoSignal<T>()
        {
            return CallbacksNoSignal.GetOrAdd(typeof(T), () => new());
        }
    }
}