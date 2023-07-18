namespace UniT.UI.Bases
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.UI.Interfaces;
    using UnityEngine;

    public abstract class BaseView : MonoBehaviour, IView
    {
        public IView.Status CurrentStatus { get; private set; } = IView.Status.Hidden;

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

        public IUIManager Manager { get; private set; }

        void IView.Initialize(IUIManager manager)
        {
            this.Manager = manager;
            this.OnInitialize();
        }

        public void Stack(bool force = false)
        {
            if (!force && this.CurrentStatus is IView.Status.Stacking) return;
            this.Hide(false, false);
            this.Manager.Stack(this);
            this.CurrentStatus = IView.Status.Stacking;
            this.OnShow();
        }

        public void Float(bool force = false)
        {
            if (!force && this.CurrentStatus is IView.Status.Floating) return;
            this.Hide(false, false);
            this.CurrentStatus = IView.Status.Floating;
            this.Manager.Float(this);
            this.OnShow();
        }

        public void Dock(bool force = false)
        {
            if (!force && this.CurrentStatus is IView.Status.Docked) return;
            this.Hide(false, false);
            this.Manager.Dock(this);
            this.CurrentStatus = IView.Status.Docked;
            this.OnShow();
        }

        public void Hide(bool removeFromStack = true, bool autoStack = true)
        {
            if (this.CurrentStatus is IView.Status.Hidden) return;
            this.Manager.Hide(this);
            this.CurrentStatus = IView.Status.Hidden;
            if (removeFromStack) this.Manager.RemoveFromStack(this);
            if (autoStack) this.Manager.StackNextView();
            this.OnHide();
            this.extras.Clear();
        }

        public void Dispose(bool autoStack = true)
        {
            this.Hide(true, autoStack);
            Destroy(this.gameObject);
            this.Manager.Dispose(this);
            this.CurrentStatus = IView.Status.Disposed;
            this.OnDispose();
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
    }

    public abstract class BaseView<TPresenter> : BaseView, IViewWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}