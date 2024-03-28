namespace UniT.UI.View
{
    using UniT.UI.UIElement;

    public interface IView : IUIElement
    {
    }

    public interface IViewWithoutParams : IView, IUIElementWithoutParams
    {
    }

    public interface IViewWithParams<TParams> : IView, IUIElementWithParams<TParams>
    {
    }
}