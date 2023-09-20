namespace UniT.UI
{
    public abstract class BasePresenter<TView> : IPresenter where TView : IViewWithPresenter
    {
        IViewWithPresenter IPresenter.View { set => this.View = (TView)value; }

        protected TView View { get; private set; }
    }
}