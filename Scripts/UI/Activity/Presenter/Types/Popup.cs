#nullable enable
namespace UniT.UI.Activity.Presenter
{
    public abstract class Popup<TPresenter> : Activity<TPresenter>, IPopup where TPresenter : IActivityPresenter
    {
    }

    public abstract class Popup<TParams, TPresenter> : Activity<TParams, TPresenter>, IPopup where TPresenter : IActivityPresenter
    {
    }
}