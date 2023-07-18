namespace UniT.UI.Item.Bases
{
    using UniT.UI.Item.Interfaces;
    using UnityEngine;

    public abstract class BaseItemView<TItem, TPresenter> : MonoBehaviour, IItemView where TPresenter : IItemPresenter
    {
        object IItemView.Item { set => this.Item = (TItem)value; }

        IItemPresenter IItemView.Presenter { set => this.Presenter = (TPresenter)value; }

        void IItemView.Initialize() => this.Initialize();

        void IItemView.Show() => this.Show();

        void IItemView.Hide() => this.Hide();

        void IItemView.Dispose() => this.Dispose();

        public TItem Item { get; private set; }

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

    public abstract class BaseItemView<TItem> : BaseItemView<TItem, NoItemPresenter>
    {
    }
}