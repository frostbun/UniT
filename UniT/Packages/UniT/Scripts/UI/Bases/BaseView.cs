namespace UniT.UI.Bases
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.UI.Interfaces;
    using UnityEngine;

    public abstract class BaseView : MonoBehaviour, IView
    {
        public IUIManager   Manager       { get; private set; }
        public IView.Status CurrentStatus { get; private set; }

        private readonly Dictionary<string, object> extras = new();

        public IView PutExtra<T>(string key, T value)
        {
            this.extras.Add(key, value);
            return this;
        }

        public T GetExtra<T>(string key)
        {
            return (T)this.extras.GetOrDefault(key);
        }

        protected virtual void OnInitialize()
        {
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

        IUIManager IView.Manager { get => this.Manager; set => this.Manager = value; }

        IView.Status IView.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        public void Stack(bool force = false) => this.Manager.Stack(this, force);

        public void Float(bool force = false) => this.Manager.Float(this, force);

        public void Dock(bool force = false) => this.Manager.Dock(this, force);

        public void Hide(bool removeFromStack = true, bool autoStack = true) => this.Manager.Hide(this, removeFromStack, autoStack);

        public void Dispose(bool autoStack = true) => this.Manager.Dispose(this, autoStack);

        void IView.OnInitialize() => this.OnInitialize();

        void IView.OnShow() => this.OnShow();

        void IView.OnHide()
        {
            this.extras.Clear();
            this.OnHide();
        }

        void IView.OnDispose() => this.OnDispose();

        #endregion
    }

    public abstract class BaseView<TPresenter> : BaseView, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}