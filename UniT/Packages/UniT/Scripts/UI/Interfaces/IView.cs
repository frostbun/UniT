namespace UniT.UI.Interfaces
{
    using UnityEngine;

    public interface IView
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }

        protected internal IContract Contract { set; }

        protected internal IPresenter Presenter { set; }

        protected internal void Initialize();

        protected internal void Show();

        protected internal void Hide();

        protected internal void Dispose();
    }
}