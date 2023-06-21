namespace Views
{
    using UniT.Data.Base;
    using UniT.Extensions;
    using UniT.UI;
    using UniT.Utils;

    public class LoadingView : BaseView<LoadingPresenter>
    {
        public override void OnShow()
        {
            this.Presenter.StartLoading();
        }

        public override void OnHide()
        {
        }
    }

    public class LoadingPresenter : BasePresenter<LoadingView>, IInitializable
    {
        private IViewManager viewManager;
        private IDataManager dataManager;

        public void Initialize()
        {
            this.viewManager = ServiceProvider<IViewManager>.Get();
            this.dataManager = ServiceProvider<IDataManager>.Get();
        }

        public async void StartLoading()
        {
            await this.dataManager.PopulateAllData();
            (await this.viewManager.GetView<HomeView, HomePresenter>()).Stack();
        }
    }
}