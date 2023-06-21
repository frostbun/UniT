namespace UniT.UI
{
    public interface IPresenter
    {
        public IView View { set; }

        public object Model { set; }
    }
}