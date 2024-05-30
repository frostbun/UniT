#nullable enable
namespace UniT.UI.Activity.Presenter
{
    public abstract class Screen<TPresenter> : Activity<TPresenter>, IScreen where TPresenter : IActivityPresenter
    {
    }

    public abstract class Screen<TParams, TPresenter> : Activity<TParams, TPresenter>, IScreen where TPresenter : IActivityPresenter
    {
    }
}