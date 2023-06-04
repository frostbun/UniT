namespace UniT.UI
{
    public interface IScreenManager
    {
        public void Open<TView, TPresenter>() where TView : IView where TPresenter : IPresenter;
    }
}