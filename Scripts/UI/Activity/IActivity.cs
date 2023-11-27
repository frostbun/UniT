namespace UniT.UI.Activity
{
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #endif

    public interface IActivity : IView
    {
        public enum Status
        {
            Stacking,
            Floating,
            Docked,
            Hidden,
            Disposed,
        }

        public Status CurrentStatus { get; set; }

        public IActivity AddExtra<T>(string key, T value);

        #if UNIT_UNITASK
        public UniTask<T> WaitForResult<T>();

        public UniTask WaitForHide();
        #endif

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }

    public static class ActivityExtensions
    {
        public static IActivity Stack(this IActivity activity, bool force = false)
        {
            return activity.Manager.Stack(activity, force);
        }

        public static IActivity Float(this IActivity activity, bool force = false)
        {
            return activity.Manager.Float(activity, force);
        }

        public static IActivity Dock(this IActivity activity, bool force = false)
        {
            return activity.Manager.Dock(activity, force);
        }

        public static void Hide(this IActivity activity, bool removeFromStack = true, bool autoStack = true)
        {
            activity.Manager.Hide(activity, removeFromStack, autoStack);
        }

        public static void Dispose(this IActivity activity, bool autoStack = true)
        {
            activity.Manager.Dispose(activity, autoStack);
        }

        #if UNIT_UNITASK
        public static UniTask<IActivity> PutExtra<T>(this UniTask<IActivity> task, string key, T value)
        {
            return task.ContinueWith(activity => activity.AddExtra(key, value));
        }

        public static UniTask<T> WaitForResult<T>(this UniTask<IActivity> task)
        {
            return task.ContinueWith(activity => activity.WaitForResult<T>());
        }

        public static UniTask WaitForHide(this UniTask<IActivity> task)
        {
            return task.ContinueWith(activity => activity.WaitForHide());
        }

        public static UniTask<IActivity> Stack(this UniTask<IActivity> task, bool force = false)
        {
            return task.ContinueWith(activity => activity.Stack(force));
        }

        public static UniTask<IActivity> Float(this UniTask<IActivity> task, bool force = false)
        {
            return task.ContinueWith(activity => activity.Float(force));
        }

        public static UniTask<IActivity> Dock(this UniTask<IActivity> task, bool force = false)
        {
            return task.ContinueWith(activity => activity.Dock(force));
        }

        public static UniTask Hide(this UniTask<IActivity> task, bool removeFromStack = true, bool autoStack = true)
        {
            return task.ContinueWith(activity => activity.Hide(removeFromStack, autoStack));
        }

        public static UniTask Dispose(this UniTask<IActivity> task, bool autoStack = true)
        {
            return task.ContinueWith(activity => activity.Dispose(autoStack));
        }
        #endif
    }
}