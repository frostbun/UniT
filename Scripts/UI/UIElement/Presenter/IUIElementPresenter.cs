namespace UniT.UI.UIElement.Presenter
{
    using UniT.UI.Presenter;

    public interface IUIElementPresenter : IPresenter
    {
        public void OnInitialize();

        public void OnShow();

        public void OnHide();

        public void OnDispose();
    }
}