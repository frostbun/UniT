namespace UniT.UI.Screen
{
    using Cysharp.Threading.Tasks;

    public static class ScreenViewExtensions
    {
        public static UniTask<IScreenView> PutExtra<T>(this UniTask<IScreenView> task, string key, T value)
        {
            return task.ContinueWith(view => view.PutExtra(key, value));
        }

        public static UniTask Stack(this UniTask<IScreenView> task, bool force = false)
        {
            return task.ContinueWith(view => view.Stack(force));
        }

        public static UniTask Float(this UniTask<IScreenView> task, bool force = false)
        {
            return task.ContinueWith(view => view.Float(force));
        }

        public static UniTask Dock(this UniTask<IScreenView> task, bool force = false)
        {
            return task.ContinueWith(view => view.Dock(force));
        }

        public static UniTask Hide(this UniTask<IScreenView> task, bool removeFromStack = true, bool autoStack = true)
        {
            return task.ContinueWith(view => view.Hide(removeFromStack, autoStack));
        }

        public static UniTask Dispose(this UniTask<IScreenView> task, bool autoStack = true)
        {
            return task.ContinueWith(view => view.Dispose(autoStack));
        }
    }
}