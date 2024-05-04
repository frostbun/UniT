namespace UniT.UI.Activity.Presenter
{
    using UniT.UI.Presenter;
    using UniT.UI.View.Presenter;

    public abstract class BaseActivityPresenter<TActivity> : BaseViewPresenter<TActivity>, IActivityPresenter where TActivity : IActivity, IHasPresenter
    {
        protected TActivity Activity => this.Owner;

        protected IActivity.Status CurrentStatus => this.Owner.CurrentStatus;

        #if UNIT_UNITASK
        protected bool SetResult(object result) => this.Owner.SetResult(result);
        #endif

        protected void Hide(bool removeFromStack = true, bool autoStack = true) => this.Owner.Hide(removeFromStack, autoStack);

        protected void Dispose(bool autoStack = true) => this.Owner.Dispose(autoStack);
    }

    public abstract class ActivityPresenter<TActivity> : BaseActivityPresenter<TActivity> where TActivity : IActivityWithoutParams, IHasPresenter
    {
    }

    public abstract class ActivityPresenter<TActivity, TParams> : BaseActivityPresenter<TActivity> where TActivity : IActivityWithParams<TParams>, IHasPresenter
    {
        protected TParams Params => this.Owner.Params;
    }
}