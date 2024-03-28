namespace UniT.UI.View.Presenter
{
    using UniT.UI.Presenter;
    using UniT.UI.UIElement.Presenter;

    public abstract class BaseViewPresenter<TView> : BaseUIElementPresenter<TView>, IViewPresenter where TView : IView, IHasPresenter
    {
        protected TView View => this.Owner;
    }

    public abstract class ViewPresenter<TView> : BaseViewPresenter<TView> where TView : IViewWithoutParams, IHasPresenter
    {
    }

    public abstract class ViewPresenter<TView, TParams> : BaseViewPresenter<TView> where TView : IViewWithParams<TParams>, IHasPresenter
    {
        protected TParams Params => this.Owner.Params;
    }
}