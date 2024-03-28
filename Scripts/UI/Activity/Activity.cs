namespace UniT.UI.Activity
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniT.UI.UIElement;

    public abstract class BaseActivity : BaseUIElement, IActivity
    {
        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        bool IActivity.IsDestroyed => !this;

        #if UNIT_UNITASK
        void IUIElement.OnHide()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnHide();
            #if UNIT_UNITASK
            this.resultSource?.TrySetResult(null);
            this.resultSource?.Task.Forget();
            this.resultSource = null;
            #endif
        }

        private UniTaskCompletionSource<object> resultSource;

        UniTask<T> IActivity.WaitForResult<T>()
        {
            return (this.resultSource ??= new UniTaskCompletionSource<object>()).Task.ContinueWith(result =>
            {
                if (result is null)
                    return default;
                if (result is not T t)
                    throw new ArgumentException($"Wrong result type. Expected {typeof(T).Name}, got {result.GetType().Name}.");
                return t;
            });
        }

        UniTask IActivity.WaitForHide()
        {
            return (this.resultSource ??= new UniTaskCompletionSource<object>()).Task;
        }

        bool IActivity.SetResult(object result)
        {
            return this.SetResult(result);
        }

        protected bool SetResult(object result)
        {
            return (this.resultSource ??= new UniTaskCompletionSource<object>()).TrySetResult(result);
        }
        #endif

        protected IActivity.Status CurrentStatus { get; private set; } = IActivity.Status.Hidden;

        public void Hide(bool removeFromStack = true, bool autoStack = true) => this.Manager.Hide(this, removeFromStack, autoStack);

        public void Dispose(bool autoStack = true) => this.Manager.Dispose(this, autoStack);
    }

    public abstract class Activity : BaseActivity, IActivityWithoutParams
    {
        public IActivity Stack(bool force = false) => this.Manager.Stack(this, force);

        public IActivity Float(bool force = false) => this.Manager.Float(this, force);

        public IActivity Dock(bool force = false) => this.Manager.Dock(this, force);
    }

    public abstract class Activity<TParams> : BaseActivity, IActivityWithParams<TParams>
    {
        TParams IUIElementWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        protected TParams Params { get; private set; }

        public IActivity Stack(TParams @params, bool force = true) => this.Manager.Stack(this, @params, force);

        public IActivity Float(TParams @params, bool force = true) => this.Manager.Float(this, @params, force);

        public IActivity Dock(TParams @params, bool force = true) => this.Manager.Dock(this, @params, force);
    }
}