#if UNIT_UNITASK
#nullable enable
namespace UniT.UI
{
    using Cysharp.Threading.Tasks;
    using UniT.UI.Activity;

    public static class ActivityExtensions
    {
        public static UniTask<IActivity> Stack<TActivity>(this UniTask<TActivity> task, bool force = false) where TActivity : IActivityWithoutParams
        {
            return task.ContinueWith(activity => activity.Stack(force));
        }

        public static UniTask<IActivity> Float<TActivity>(this UniTask<TActivity> task, bool force = false) where TActivity : IActivityWithoutParams
        {
            return task.ContinueWith(activity => activity.Float(force));
        }

        public static UniTask<IActivity> Dock<TActivity>(this UniTask<TActivity> task, bool force = false) where TActivity : IActivityWithoutParams
        {
            return task.ContinueWith(activity => activity.Dock(force));
        }

        public static UniTask<IActivity> Stack<TActivity, TParams>(this UniTask<TActivity> task, TParams @params, bool force = true) where TActivity : IActivityWithParams<TParams>
        {
            return task.ContinueWith(activity => activity.Stack(@params, force));
        }

        public static UniTask<IActivity> Float<TActivity, TParams>(this UniTask<TActivity> task, TParams @params, bool force = true) where TActivity : IActivityWithParams<TParams>
        {
            return task.ContinueWith(activity => activity.Float(@params, force));
        }

        public static UniTask<IActivity> Dock<TActivity, TParams>(this UniTask<TActivity> task, TParams @params, bool force = true) where TActivity : IActivityWithParams<TParams>
        {
            return task.ContinueWith(activity => activity.Dock(@params, force));
        }

        public static UniTask<T?> WaitForResult<T>(this UniTask<IActivity> task)
        {
            return task.ContinueWith(activity => activity.WaitForResult<T>());
        }

        public static UniTask WaitForHide(this UniTask<IActivity> task)
        {
            return task.ContinueWith(activity => activity.WaitForHide());
        }
    }
}
#endif