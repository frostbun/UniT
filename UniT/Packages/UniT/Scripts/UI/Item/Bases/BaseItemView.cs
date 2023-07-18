namespace UniT.UI.Item.Bases
{
    using System;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;

    public abstract class BaseItemView<TItem> : MonoBehaviour, IItemView
    {
        void IItemView.Initialize()
        {
            this.OnInitialize();
        }

        void IItemView.Show(object item)
        {
            this.transform.SetAsLastSibling();
            this.gameObject.SetActive(true);
            this.Item = (TItem)item;
            this.OnShow();
        }

        void IItemView.Hide()
        {
            this.gameObject.SetActive(false);
            this.OnHide();
        }

        void IItemView.Dispose()
        {
            Destroy(this.gameObject);
            this.OnDispose();
        }

        public TItem Item { get; private set; }

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

    public abstract class BaseItemView<TItem, TPresenter> : BaseItemView<TItem>, IItemViewWithPresenter where TPresenter : IItemPresenter
    {
        Type IItemViewWithPresenter.PresenterType => typeof(TPresenter);

        IItemPresenter IItemViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected TPresenter Presenter { get; private set; }
    }
}