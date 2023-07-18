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

        public Status CurrentStatus { get; }

        public IView PutExtra<T>(string key, T value);

        public T GetExtra<T>(string key);

        protected internal void Initialize(IUIManager manager);

        public void Stack(bool force = false);

        public void Float(bool force = false);

        public void Dock(bool force = false);

        public void Hide(bool removeFromStack = true, bool autoStack = true);

        public void Dispose(bool autoStack = true);

        public Transform transform { get; }
    }

    public interface IViewWithPresenter : IView
    {
        protected internal Type PresenterType { get; }

        protected internal IPresenter Presenter { set; }
    }
}