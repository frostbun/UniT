namespace UniT.UI.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public abstract class BaseActivity : BaseView, IActivity
    {
        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        void IActivity.OnShow()
        {
            this._currentExtras = this._nextExtras;
            this._nextExtras    = null;
            this.OnShow();
        }

        void IActivity.OnHide()
        {
            this._hideCts?.Cancel();
            this._hideCts?.Dispose();
            this._hideCts = null;
            this.OnHide();
            this._currentExtras = null;
            this._resultSource?.TrySetResult(null);
            this._resultSource?.Task.Forget();
            this._resultSource = null;
        }

        void IActivity.OnDispose() => this.OnDispose();

        private Dictionary<string, object>      _currentExtras;
        private Dictionary<string, object>      _nextExtras;
        private UniTaskCompletionSource<object> _resultSource;
        private CancellationTokenSource         _hideCts;

        IActivity IActivity.AddExtra<T>(string key, T value)
        {
            this._nextExtras ??= new();
            if (this._nextExtras.ContainsKey(key))
                throw new ArgumentException($"Extra with key {key} already exists");
            this._nextExtras[key] = value;
            return this;
        }

        UniTask<T> IActivity.WaitForResult<T>()
        {
            return (this._resultSource ??= new()).Task.ContinueWith(result =>
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
            return (this._resultSource ??= new()).Task;
        }

        public IActivity.Status CurrentStatus { get; private set; } = IActivity.Status.Hidden;

        public T GetExtra<T>(string key)
        {
            if (this._currentExtras is null || !this._currentExtras.ContainsKey(key))
                throw new ArgumentException($"No extra with key {key} found");
            var extra = this._currentExtras[key];
            if (extra is null)
                return default;
            if (extra is not T t)
                throw new ArgumentException($"Found an extra with key {key} but wrong type. Expected {typeof(T).Name}, got {extra.GetType().Name}.");
            return t;
        }

        public bool TrySetResult<T>(T result)
        {
            return (this._resultSource ??= new()).TrySetResult(result);
        }

        public CancellationToken GetCancellationTokenOnHide()
        {
            return (this._hideCts ??= new()).Token;
        }

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

    public abstract class BaseActivity<TPresenter> : BaseActivity, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}