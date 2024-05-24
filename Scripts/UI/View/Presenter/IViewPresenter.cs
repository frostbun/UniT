#nullable enable
namespace UniT.UI.View.Presenter
{
    using UniT.UI.Presenter;

    public interface IViewPresenter : IPresenter
    {
        public void OnInitialize();

        public void OnShow();

        public void OnHide();
    }
}