namespace UniT.UI
{
    public interface IPresenter
    {
        protected internal IContract Contract { set; }

        protected internal IView View { set; }
    }
}