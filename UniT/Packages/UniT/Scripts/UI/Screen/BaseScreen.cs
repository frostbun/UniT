namespace UniT.UI.Screen
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public abstract class BaseScreen : BaseView, IScreen
    {
        IScreen.Status IScreen.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        void IView.OnHide()
        {
            this._extras.Clear();
            this.OnHide();
        }

        public IScreen.Status CurrentStatus { get; private set; } = IScreen.Status.Hidden;

        private readonly Dictionary<string, object> _extras = new();

        public IScreen PutExtra<T>(string key, T value)
        {
            this._extras.Add(key, value);
            return this;
        }

        public T GetExtra<T>(string key)
        {
            return (T)this._extras.GetOrDefault(key);
        }
    }

    public abstract class BaseScreen<TPresenter> : BaseScreen, IScreenWithPresenter where TPresenter : IPresenter
    {
        Type IViewWithPresenter.PresenterType => this.PresenterType;

        IPresenter IViewWithPresenter.Presenter { set => this.Presenter = (TPresenter)value; }

        protected virtual Type PresenterType => typeof(TPresenter);

        protected TPresenter Presenter { get; private set; }
    }
}