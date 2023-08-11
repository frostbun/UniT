namespace UniT.UI.Activity
{
    using System.Runtime.CompilerServices;
    using Cysharp.Threading.Tasks;

    public static class ActivityExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask<IActivity> PutExtra<T>(this UniTask<IActivity> task, string key, T value)
        {
            return task.ContinueWith(activity => activity.PutExtra(key, value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask Stack(this UniTask<IActivity> task, bool force = false)
        {
            return task.ContinueWith(activity => activity.Stack(force));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask Float(this UniTask<IActivity> task, bool force = false)
        {
            return task.ContinueWith(activity => activity.Float(force));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask Dock(this UniTask<IActivity> task, bool force = false)
        {
            return task.ContinueWith(activity => activity.Dock(force));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask Hide(this UniTask<IActivity> task, bool removeFromStack = true, bool autoStack = true)
        {
            return task.ContinueWith(activity => activity.Hide(removeFromStack, autoStack));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask Dispose(this UniTask<IActivity> task, bool autoStack = true)
        {
            return task.ContinueWith(activity => activity.Dispose(autoStack));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Stack(this IActivity activity, bool force = false)
        {
            activity.Manager.Stack(activity, force);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Float(this IActivity activity, bool force = false)
        {
            activity.Manager.Float(activity, force);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dock(this IActivity activity, bool force = false)
        {
            activity.Manager.Dock(activity, force);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Hide(this IActivity activity, bool removeFromStack = true, bool autoStack = true)
        {
            activity.Manager.Hide(activity, removeFromStack, autoStack);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dispose(this IActivity activity, bool autoStack = true)
        {
            activity.Manager.Dispose(activity, autoStack);
        }
    }
}