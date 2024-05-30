#nullable enable
namespace UniT.UI.Activity.Presenter
{
    public abstract class Overlay<TPresenter> : Activity<TPresenter>, IOverlay where TPresenter : IActivityPresenter
    {
    }

    public abstract class Overlay<TParams, TPresenter> : Activity<TParams, TPresenter>, IOverlay where TPresenter : IActivityPresenter
    {
    }
}