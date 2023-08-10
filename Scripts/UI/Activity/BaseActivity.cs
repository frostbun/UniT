namespace UniT.UI.Activity
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public abstract class BaseActivity : BaseView, IActivity
    {
        public IActivity.Status CurrentStatus { get; private set; } = IActivity.Status.Hidden;

        private readonly Dictionary<string, object> _extras = new();

        public IActivity PutExtra<T>(string key, T value)
        {
            this._extras[key] = value;
            return this;
        }

        public T GetExtra<T>(string key)
        {
            return (T)this._extras.GetOrDefault(key);
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

        #region Interface Implementation

        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        void IActivity.OnShow() => this.OnShow();

        void IActivity.OnHide()
        {
            this._extras.Clear();
            this.OnHide();
        }

        void IActivity.OnDispose() => this.OnDispose();

        #endregion
    }

    public abstract class BaseActivity<TPresenter> : BaseActivity, IActivityWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}