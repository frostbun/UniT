namespace UniT.UI
{
    using Cysharp.Threading.Tasks;

    public interface IContract
    {
        public enum Status
        {
            Stacking,
            Floating,
            Docked,
            Hidden,
            Disposed,
        }

        public Status CurrentStatus { get; }

        public IContract PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        public void Stack(bool force = false);

        public void Float(bool force = false);

        public void Dock(bool force = false);

        public void Hide(bool autoStack = true);

        public void Dispose(bool autoStack = true);
    }

    public static class ContractExtensions
    {
        public static UniTask<IContract> PutExtra<T>(this UniTask<IContract> task, string key, T value)
        {
            return task.ContinueWith(contract => contract.PutExtra(key, value));
        }

        public static UniTask Stack(this UniTask<IContract> task, bool force = false)
        {
            return task.ContinueWith(contract => contract.Stack(force));
        }

        public static UniTask Float(this UniTask<IContract> task, bool force = false)
        {
            return task.ContinueWith(contract => contract.Float(force));
        }

        public static UniTask Dock(this UniTask<IContract> task, bool force = false)
        {
            return task.ContinueWith(contract => contract.Dock(force));
        }

        public static UniTask Hide(this UniTask<IContract> task, bool autoStack = true)
        {
            return task.ContinueWith(contract => contract.Hide(autoStack));
        }

        public static UniTask Dispose(this UniTask<IContract> task, bool autoStack = true)
        {
            return task.ContinueWith(contract => contract.Dispose(autoStack));
        }
    }
}