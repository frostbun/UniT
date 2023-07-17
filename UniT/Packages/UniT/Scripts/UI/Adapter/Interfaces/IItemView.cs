namespace UniT.UI.Adapter.Interfaces
{
    using UnityEngine;

    public interface IItemView<TItem>
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }

        protected internal IItemContract<TItem> Contract { set; }

        protected internal IItemPresenter<TItem> Presenter { set; }

        protected internal void Initialize();

        protected internal void Show();

        protected internal void Hide();

        protected internal void Dispose();
    }
}