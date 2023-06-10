namespace UniT.Core.Utils
{
    using System;
    using System.Collections.Generic;

    public static class SignalBus<T> where T : class
    {
        private static readonly HashSet<Action<T>> callbackWithSignal = new();
        private static readonly HashSet<Action>    callbackNoSignal   = new();

        public static void Fire(T signal)
        {
            foreach (var action in callbackWithSignal)
            {
                action.Invoke(signal);
            }

            foreach (var action in callbackNoSignal)
            {
                action.Invoke();
            }
        }

        public static void Subscribe(Action<T> callback)
        {
            callbackWithSignal.Add(callback);
        }

        public static void Subscribe(Action callback)
        {
            callbackNoSignal.Add(callback);
        }

        public static void Unsubscribe(Action<T> callback)
        {
            callbackWithSignal.Remove(callback);
        }

        public static void Unsubscribe(Action callback)
        {
            callbackNoSignal.Remove(callback);
        }
    }
}