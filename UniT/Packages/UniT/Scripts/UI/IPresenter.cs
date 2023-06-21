namespace UniT.UI
{
    public interface IPresenter
    {
        protected internal IView View { set; }

        protected internal object Model { set; }

        protected internal void Initialize();

        protected internal void Dispose();
    }
}