namespace UniT.UI
{
    using System;
    using UniT.Utilities;

    public interface IPresenter : IInitializable, IDisposable
    {
        protected internal IView View { set; }

        protected internal object Model { set; }
    }
}