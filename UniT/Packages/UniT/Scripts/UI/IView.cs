namespace UniT.UI
{
    using System;
    using UniT.Utilities;
    using UnityEngine;

    public interface IView : IInitializable, IDisposable
    {
        protected internal GameObject gameObject { get; }

        protected internal Transform transform { get; }

        protected internal IPresenter Presenter { set; }

        protected internal IViewManager.IViewInstance Instance { set; }

        protected internal void OnShow();

        protected internal void OnHide();
    }
}