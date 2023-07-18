namespace UniT.UI.Bases
{
    using UniT.UI.Interfaces;

    public abstract class BasePresenter<TView> : IPresenter where TView : IViewWithPresenter
    {
        IView IPresenter.View { set => this.View = (TView)value; }

        protected TView View { get; private set; }
    }
}