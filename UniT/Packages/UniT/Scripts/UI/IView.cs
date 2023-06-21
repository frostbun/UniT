namespace UniT.UI
{
    using UnityEngine;

    public interface IView
    {
        public GameObject gameObject { get; }

        public Transform transform { get; }

        public IPresenter Presenter { set; }

        public void OnShow();

        public void OnHide();
    }
}