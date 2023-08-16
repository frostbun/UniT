namespace UniT.UI.Activity
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;

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
            this._extras.Clear();
            this._resultSource = null;
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
                if (result is not T t)
                    throw new ArgumentException($"Wrong result type. Expected {typeof(T).Name}, got {result.GetType().Name}.");
                return t;
            });
        }

        public T GetExtra<T>(string key)
        {
            var extra = this._extras.GetOrDefault(key)
                ?? throw new ArgumentException($"No extra with key {key} found");
            if (extra is not T t)
                throw new ArgumentException($"Found an extra with key {key} but wrong type. Expected {typeof(T).Name}, got {extra.GetType().Name}.");
            return t;
        }

        public void SetResult<T>(T result)
        {
            this._resultSource.TrySetResult(result);
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

    public abstract class BaseActivity<TPresenter> : BaseActivity, IActivityWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}