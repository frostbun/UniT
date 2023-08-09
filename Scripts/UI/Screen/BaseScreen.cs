namespace UniT.UI.Screen
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public abstract class BaseScreen : BaseView, IScreen
    {
        public IScreen.Status CurrentStatus { get; private set; } = IScreen.Status.Hidden;

        private readonly Dictionary<string, object> _extras = new();

        public IScreen PutExtra<T>(string key, T value)
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

        IScreen.Status IScreen.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        void IScreen.OnShow() => this.OnShow();

        void IScreen.OnHide()
        {
            this._extras.Clear();
            this.OnHide();
        }

        void IScreen.OnDispose() => this.OnDispose();

        #endregion
    }

    public abstract class BaseScreen<TPresenter> : BaseScreen, IScreenWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}