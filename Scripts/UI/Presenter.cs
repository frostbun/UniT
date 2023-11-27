namespace UniT.UI
{
    public abstract class Presenter<TView> : IPresenter where TView : IView, IHasPresenter
    {
        IHasPresenter IPresenter.Owner { set => this.View = (TView)value; }

        protected TView View { get; private set; }
    }
}