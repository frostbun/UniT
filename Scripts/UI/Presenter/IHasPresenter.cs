namespace UniT.UI
{
    using System;

    public interface IHasPresenter
    {
        public Type PresenterType { get; }

        public IPresenter Presenter { set; }
    }
}