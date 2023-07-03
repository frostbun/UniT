namespace UniT.UI
{
    using System;
    using UniT.Utilities;

    public abstract class BasePresenter<TView> : BasePresenter<TView, object> where TView : IView
    {
    }

    public abstract class BasePresenter<TView, TModel> : IPresenter where TView : IView
    {
        IView IPresenter.View { set => this.View = (TView)value; }

        object IPresenter.Model { set => this.Model = (TModel)value; }

        void IInitializable.Initialize() => this.Initialize();

        void IDisposable.Dispose() => this.Dispose();

        protected TView View { get; private set; }

        protected TModel Model { get; private set; }

        protected virtual void Initialize()
        {
        }

        protected virtual void Dispose()
        {
        }
    }
}