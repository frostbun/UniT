namespace UniT.UI.Activity
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public abstract class BaseActivity : BaseView, IActivity
    {
        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        void IActivity.OnShow()
        {
            this._resultSource = new();
            this.OnShow();
        }

        void IActivity.OnHide()
        {
            this.OnHide();
            this._resultSource.TrySetResult(null);
            this._resultSource = null;
            this._extras.Clear();
        }

        void IActivity.OnDispose() => this.OnDispose();

        public IActivity.Status CurrentStatus { get; private set; } = IActivity.Status.Hidden;

        private readonly Dictionary<string, object>      _extras = new();
        private          UniTaskCompletionSource<object> _resultSource;

        IActivity IActivity.PutExtra<T>(string key, T value)
        {
            if (!this._extras.TryAdd(key, value))
                throw new ArgumentException($"Duplicate key {key} found");
            return this;
        }

        UniTask<T> IActivity.WaitForResult<T>()
        {
            if (this._resultSource is null)
                throw new InvalidOperationException("Activity must be shown before wait for result");
            return this._resultSource.Task.ContinueWith(result =>
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
            if (this._resultSource is null)
                throw new InvalidOperationException("Activity must be shown before wait for hide");
            return this._resultSource.Task;
        }

        public T GetExtra<T>(string key)
        {
            if (!this._extras.ContainsKey(key))
                throw new ArgumentException($"No extra with key {key} found");
            var extra = this._extras[key];
            if (extra is null)
                return default;
            if (extra is not T t)
                throw new ArgumentException($"Found an extra with key {key} but wrong type. Expected {typeof(T).Name}, got {extra.GetType().Name}.");
            return t;
        }

        public void SetResult<T>(T result)
        {
            if (this._resultSource is null)
                throw new InvalidOperationException("Activity must be shown before set result");
            if (!this._resultSource.TrySetResult(result))
                throw new InvalidOperationException("Result already set");
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