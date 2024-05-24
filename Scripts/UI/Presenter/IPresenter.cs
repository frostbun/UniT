#nullable enable
namespace UniT.UI.Presenter
{
    public interface IPresenter
    {
        public IHasPresenter Owner { set; }
    }
}