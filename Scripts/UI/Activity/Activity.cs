#nullable enable
namespace UniT.UI.Activity
{
    using UniT.UI.View;

    public abstract class BaseActivity : BaseView, IActivity
    {
        IActivity.Status IActivity.CurrentStatus { get => this.CurrentStatus; set => this.CurrentStatus = value; }

        public IActivity.Status CurrentStatus { get; private set; }

        public bool IsDestroyed => !this;

        public void Hide(bool autoStack = true) => this.Manager.Hide(this, autoStack);

        public void Dispose(bool autoStack = true) => this.Manager.Dispose(this, autoStack);
    }

    public abstract class Activity : BaseActivity, IActivityWithoutParams
    {
        public void Show(bool force = false) => this.Manager.Show(this, force);
    }

    public abstract class Activity<TParams> : BaseActivity, IActivityWithParams<TParams>
    {
        TParams IViewWithParams<TParams>.Params { get => this.Params; set => this.Params = value; }

        public TParams Params { get; private set; } = default!;

        public void Show(TParams @params, bool force = true) => this.Manager.Show(this, @params, force);
    }
}