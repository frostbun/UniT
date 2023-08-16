namespace UniT.UI
{
    using System;
    using UnityEngine;

    public interface IView
    {
        public IUIManager Manager { get; set; }

        public void OnInitialize();

        public GameObject gameObject { get; }

        public Transform transform { get; }
    }

    public interface IViewWithPresenter : IView
    {
        public Type PresenterType { get; }

        public IPresenter Presenter { set; }
    }
}