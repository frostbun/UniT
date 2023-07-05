namespace UniT.UI
{
    using System;
    using UniT.Utilities;
    using UnityEngine;

    public interface IView : IInitializable, IDisposable
    {
        protected internal GameObject GameObject { get; }

        protected internal Transform Transform { get; }

        protected internal IContract Contract { set; }

        protected internal IPresenter Presenter { set; }

        protected internal void OnShow();

        protected internal void OnHide();
    }
}