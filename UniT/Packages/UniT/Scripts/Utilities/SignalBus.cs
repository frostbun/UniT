namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public static class SignalBus<T> where T : class
    {
        private static readonly HashSet<Action<T>> callbacksWithSignal = new();
        private static readonly HashSet<Action>    callbacksNoSignal   = new();

        public static void Fire(T signal)
        {
            callbacksWithSignal.ForEach(callback => callback(signal));
            callbacksNoSignal.ForEach(callback => callback());
        }

        public static void Subscribe(Action<T> callback)
        {
            callbacksWithSignal.Add(callback);
        }

        public static void Subscribe(Action callback)
        {
            callbacksNoSignal.Add(callback);
        }

        public static void Unsubscribe(Action<T> callback)
        {
            callbacksWithSignal.Remove(callback);
        }

        public static void Unsubscribe(Action callback)
        {
            callbacksNoSignal.Remove(callback);
        }
    }
}