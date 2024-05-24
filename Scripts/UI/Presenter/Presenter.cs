#nullable enable
namespace UniT.UI.Presenter
{
    public abstract class Presenter<TOwner> : IPresenter where TOwner : IHasPresenter
    {
        IHasPresenter IPresenter.Owner { set => this.Owner = (TOwner)value; }

        protected TOwner Owner { get; private set; } = default!;
    }
}