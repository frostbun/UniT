namespace UniT.UI.Activity
{
    using System;
    using System.Collections.Generic;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class Activity : View, IActivity
    {
        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        void IActivity.OnShow()
        {
            this.currentExtras = this.nextExtras;
            this.nextExtras    = null;
            this.OnShow();
        }

        void IActivity.OnHide()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnHide();
            this.currentExtras = null;
            #if UNIT_UNITASK
            this.resultSource?.TrySetResult(null);
            this.resultSource?.Task.Forget();
            this.resultSource = null;
            #endif
        }

        void IActivity.OnDispose() => this.OnDispose();

        protected IActivity.Status CurrentStatus { get; private set; } = IActivity.Status.Hidden;

        private Dictionary<string, object> currentExtras;
        private Dictionary<string, object> nextExtras;

        IActivity IActivity.AddExtra<T>(string key, T value)
        {
            this.nextExtras ??= new();
            if (this.nextExtras.ContainsKey(key))
                throw new ArgumentException($"Extra with key {key} already exists");
            this.nextExtras[key] = value;
            return this;
        }

        protected T GetExtra<T>(string key)
        {
            if (this.currentExtras is null || !this.currentExtras.ContainsKey(key))
                throw new ArgumentException($"No extra with key {key} found");
            var extra = this.currentExtras[key];
            if (extra is null)
                return default;
            if (extra is not T t)
                throw new ArgumentException($"Found an extra with key {key} but wrong type. Expected {typeof(T).Name}, got {extra.GetType().Name}.");
            return t;
        }

        #if UNIT_UNITASK
        private UniTaskCompletionSource<object> resultSource;

        UniTask<T> IActivity.WaitForResult<T>()
        {
            return (this.resultSource ??= new()).Task.ContinueWith(result =>
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
            return (this.resultSource ??= new()).Task;
        }

        protected bool TrySetResult<T>(T result)
        {
            return (this.resultSource ??= new()).TrySetResult(result);
        }

        private CancellationTokenSource hideCts;

        protected CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new()).Token;
        }
        #endif

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnDispose()
        {
        }
    }

    public abstract class Activity<TPresenter> : Activity, IHasPresenter where TPresenter : IPresenter
    {
        Type IHasPresenter.PresenterType => this.PresenterType;

        IPresenter IHasPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType { get; } = typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}