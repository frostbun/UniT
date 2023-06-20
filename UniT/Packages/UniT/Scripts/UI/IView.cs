namespace UniT.UI
{
    using UnityEngine;

    public interface IView
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }

        public IPresenter Presenter { set; }

        public void OnInitialize();

        public void OnShow();

        public void OnHide();

        public void OnClose();
    }
}