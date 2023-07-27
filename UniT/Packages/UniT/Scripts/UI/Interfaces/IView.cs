namespace UniT.UI.Interfaces
{
    using System;
    using UnityEngine;

    public interface IView
    {
        public enum Status
        {
            Stacking,
            Floating,
            Docked,
            Hidden,
            Disposed,
        }

        public GameObject gameObject { get; }

        public Transform transform { get; }

        public IUIManager Manager { get; protected internal set; }

        public Status CurrentStatus { get; protected internal set; }

        public IView PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        public void Stack(bool force = false);

        public void Float(bool force = false);

        public void Dock(bool force = false);

        public void Hide(bool removeFromStack = true, bool autoStack = true);

        public void Dispose(bool autoStack = true);

        protected internal void OnInitialize();

        protected internal void OnShow();

        protected internal void OnHide();

        protected internal void OnDispose();
    }

    public interface IViewWithPresenter : IView
    {
        protected internal Type PresenterType { get; }

        protected internal IPresenter Presenter { set; }
    }
}