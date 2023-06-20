namespace UniT.UI
{
    public interface IPresenter
    {
        public IView View { set; }

        public object Model { set; }

        public void OnInitialize();

        public void OnShow();

        public void OnHide();

        public void OnClose();
    }
}