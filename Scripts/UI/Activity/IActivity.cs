namespace UniT.UI.Activity
{
    using Cysharp.Threading.Tasks;

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

        public IActivity PutExtra<T>(string key, T value);

        public UniTask<T> WaitForResult<T>();

        public T GetExtra<T>(string key);

        public void SetResult<T>(T result);

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }
}