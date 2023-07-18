namespace UniT.UI.Item.Interfaces
{
    using System;
    using UnityEngine;

    public interface IItemView
    {
        protected internal void Initialize();

        protected internal void Show(object item);

        protected internal void Hide();

        protected internal void Dispose();

        public Transform transform { get; }
    }

    public interface IItemViewWithPresenter : IItemView
    {
        protected internal Type PresenterType { get; }

        protected internal IItemPresenter Presenter { set; }
    }
}