namespace UniT.UI.Item.Interfaces
{
    public interface IItemAdapter
    {
        protected internal void Initialize(IUIManager manager, IItemPresenter.Factory itemPresenterFactory);
    }
}