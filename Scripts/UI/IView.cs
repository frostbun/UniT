namespace UniT.UI
{
    using System;
    using UnityEngine;

    public interface IView
    {
        public IUIManager Manager { get; protected internal set; }

        protected internal void OnInitialize();

        protected internal void OnShow();

        protected internal void OnHide();

        protected internal void OnDispose();

        public GameObject gameObject { get; }

        public Transform transform { get; }
    }

    public interface IViewWithPresenter : IView
    {
        protected internal Type PresenterType { get; }

        protected internal IPresenter Presenter { set; }
    }
}