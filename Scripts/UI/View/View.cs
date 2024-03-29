namespace UniT.UI.View
{
    using UniT.UI.UIElement;

    public abstract class BaseView : BaseUIElement, IView
    {
    }

    public abstract class View : BaseView, IViewWithoutParams
    {
    }

    public abstract class View<TParams> : BaseView, IViewWithParams<TParams>
    {
        TParams IUIElementWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        public TParams Params { get; private set; }
    }
}