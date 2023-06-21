namespace Views
{
    using UniT.Data.Base;
    using UniT.Extensions;
    using UniT.UI;
    using UniT.Utils;

    public class LoadingView : BaseView<LoadingPresenter>
    {
        protected override void OnShow()
        {
            this.Presenter.StartLoading();
        }

    }

    public class LoadingPresenter : BasePresenter<LoadingView>
    {
        private IViewManager viewManager;
        private IDataManager dataManager;

        protected override void Initialize()
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