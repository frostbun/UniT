namespace UniT.UI.Screen
{
    using Cysharp.Threading.Tasks;

    public static class ScreenExtensions
    {
        public static UniTask<IScreen> PutExtra<T>(this UniTask<IScreen> task, string key, T value)
        {
            return task.ContinueWith(screen => screen.PutExtra(key, value));
        }

        public static UniTask Stack(this UniTask<IScreen> task, bool force = false)
        {
            return task.ContinueWith(screen => screen.Stack(force));
        }

        public static UniTask Float(this UniTask<IScreen> task, bool force = false)
        {
            return task.ContinueWith(screen => screen.Float(force));
        }

        public static UniTask Dock(this UniTask<IScreen> task, bool force = false)
        {
            return task.ContinueWith(screen => screen.Dock(force));
        }

        public static UniTask Hide(this UniTask<IScreen> task, bool removeFromStack = true, bool autoStack = true)
        {
            return task.ContinueWith(screen => screen.Hide(removeFromStack, autoStack));
        }

        public static UniTask Dispose(this UniTask<IScreen> task, bool autoStack = true)
        {
            return task.ContinueWith(screen => screen.Dispose(autoStack));
        }

        public static void Stack(this IScreen screen, bool force = false)
        {
            screen.Manager.Stack(screen, force);
        }

        public static void Float(this IScreen screen, bool force = false)
        {
            screen.Manager.Float(screen, force);
        }

        public static void Dock(this IScreen screen, bool force = false)
        {
            screen.Manager.Dock(screen, force);
        }

        public static void Hide(this IScreen screen, bool removeFromStack = true, bool autoStack = true)
        {
            screen.Manager.Hide(screen, removeFromStack, autoStack);
        }

        public static void Dispose(this IScreen screen, bool autoStack = true)
        {
            screen.Manager.Dispose(screen, autoStack);
        }
    }
}