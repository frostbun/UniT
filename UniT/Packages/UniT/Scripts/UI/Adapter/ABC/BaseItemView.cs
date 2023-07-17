namespace UniT.UI.Adapter.ABC
{
    using UniT.UI.Adapter.Interfaces;
    using UnityEngine;

    public abstract class BaseItemView<TItem, TPresenter> : MonoBehaviour, IItemView<TItem> where TPresenter : IItemPresenter<TItem>
    {
        IItemContract<TItem> IItemView<TItem>.Contract { set => this.Contract = value; }

        IItemPresenter<TItem> IItemView<TItem>.Presenter { set => this.Presenter = (TPresenter)value; }

        void IItemView<TItem>.Initialize() => this.Initialize();

        void IItemView<TItem>.Show() => this.Show();

        void IItemView<TItem>.Hide() => this.Hide();

        void IItemView<TItem>.Dispose() => this.Dispose();

        protected IItemContract<TItem> Contract { get; private set; }

        protected TPresenter Presenter { get; private set; }

        protected virtual void Initialize()
        {
        }

        protected virtual void Show()
        {
        }

        protected virtual void Hide()
        {
        }

        protected virtual void Dispose()
        {
        }
    }
}