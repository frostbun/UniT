namespace UniT.UI
{
    using UnityEngine;

    public abstract class BaseView<TPresenter> : MonoBehaviour, IView where TPresenter : IPresenter
    {
        IPresenter IView.Presenter
        {
            set => this.Presenter = (TPresenter)value;
        }

        protected TPresenter Presenter { get; private set; }

        public abstract void OnShow();

        public abstract void OnHide();
    }
}