namespace UniT.UI
{
    public abstract class BasePresenter<TView> : IPresenter where TView : IView
    {
        IContract IPresenter.Contract { set => this.Contract = value; }

        IView IPresenter.View { set => this.View = (TView)value; }

        protected IContract Contract { get; private set; }

        protected TView View { get; private set; }
    }
}