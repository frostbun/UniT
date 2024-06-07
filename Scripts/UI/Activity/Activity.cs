#nullable enable
namespace UniT.UI.Activity
{
    using UniT.UI.View;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class BaseActivity : BaseView, IActivity
    {
        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        public IActivity.Status CurrentStatus { get; private set; }

        public bool IsDestroyed => !this;

        #if UNIT_UNITASK
        void IView.OnHide()
        {
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            this.OnHide();
            this.resultSource?.TrySetResult(null);
            this.resultSource?.Task.Forget();
            this.resultSource = null;
        }

        private UniTaskCompletionSource<object?>? resultSource;

        public UniTask<T> WaitForResult<T>()
        {
            return (this.resultSource ??= new UniTaskCompletionSource<object?>()).Task.ContinueWith(result => (T)result!);
        }

        public UniTask WaitForHide()
        {
            return (this.resultSource ??= new UniTaskCompletionSource<object?>()).Task;
        }

        bool IActivity.SetResult(object? result)
        {
            return this.SetResult(result);
        }

        protected bool SetResult(object? result)
        {
            return (this.resultSource ??= new UniTaskCompletionSource<object?>()).TrySetResult(result);
        }
        #endif

        public void Hide(bool autoStack = true) => this.Manager.Hide(this, autoStack);

        public void Dispose(bool autoStack = true) => this.Manager.Dispose(this, autoStack);
    }

    public abstract class Activity : BaseActivity, IActivityWithoutParams
    {
        public void Show(bool force = false) => this.Manager.Show(this, force);
    }

    public abstract class Activity<TParams> : BaseActivity, IActivityWithParams<TParams>
    {
        TParams IViewWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        public TParams Params { get; private set; } = default!;

        public void Show(TParams @params, bool force = true) => this.Manager.Show(this, @params, force);
    }
}