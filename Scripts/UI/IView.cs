namespace UniT.UI
{
    using System;
    using UnityEngine;

    public interface IView
    {
        public IUIManager Manager { get; protected internal set; }

        protected internal void OnInitialize();

        protected internal GameObject GameObject { get; }

        protected internal Transform Transform { get; }
    }

    public interface IViewWithPresenter : IView
    {
        protected internal Type PresenterType { get; }

        protected internal IPresenter Presenter { set; }
    }
}