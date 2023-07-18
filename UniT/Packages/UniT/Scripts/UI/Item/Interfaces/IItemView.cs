namespace UniT.UI.Item.Interfaces
{
    using UnityEngine;

    public interface IItemView
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }

        protected internal object Item { set; }

        protected internal IItemPresenter Presenter { set; }

        protected internal void Initialize();

        protected internal void Show();

        protected internal void Hide();

        protected internal void Dispose();
    }
}