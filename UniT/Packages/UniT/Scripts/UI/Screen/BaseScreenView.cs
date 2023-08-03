namespace UniT.UI.Screen
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public abstract class BaseScreenView : BaseView, IScreenView
    {
        public IScreenView.Status CurrentStatus { get; private set; }

        private readonly Dictionary<string, object> _extras = new();

        public IScreenView PutExtra<T>(string key, T value)
        {
            this._extras.Add(key, value);
            return this;
        }

        public T GetExtra<T>(string key)
        {
            return (T)this._extras.GetOrDefault(key);
        }

        #region Interface Implementation

        IScreenView.Status IScreenView.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        public void Stack(bool force = false) => this.Manager.Stack(this, force);

        public void Float(bool force = false) => this.Manager.Float(this, force);

        public void Dock(bool force = false) => this.Manager.Dock(this, force);

        public void Hide(bool removeFromStack = true, bool autoStack = true) => this.Manager.Hide(this, removeFromStack, autoStack);

        public void Dispose(bool autoStack = true) => this.Manager.Dispose(this, autoStack);

        void IView.OnHide()
        {
            this._extras.Clear();
            this.OnHide();
        }

        #endregion
    }

    public abstract class BaseScreenView<TPresenter> : BaseScreenView, IScreenViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}