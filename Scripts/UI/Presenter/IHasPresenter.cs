#nullable enable
namespace UniT.UI.Presenter
{
    using System;

    public interface IHasPresenter
    {
        public Type PresenterType { get; }

        public IPresenter Presenter { set; }
    }
}