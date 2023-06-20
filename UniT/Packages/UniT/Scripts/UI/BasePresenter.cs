namespace UniT.UI
{
    public abstract class BasePresenter<TView> : BasePresenter<TView, object> where TView : IView
    {
    }

    public abstract class BasePresenter<TView, TModel> : IPresenter where TView : IView
    {
        IView IPresenter.View
        {
            set => this.View = (TView)value;
        }

        object IPresenter.Model
        {
            set => this.Model = (TModel)value;
        }

        protected TView View { get; private set; }

        protected TModel Model { get; private set; }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }

        public virtual void OnClose()
        {
        }
    }
}