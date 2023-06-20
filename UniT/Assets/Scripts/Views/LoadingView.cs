namespace Views
{
    using Cysharp.Threading.Tasks;
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.UI;
    using UniT.Utils;

    public class LoadingView : BaseView<LoadingPresenter>
    {
        public override void OnShow()
        {
            this.Presenter.StartLoading();
        }
    }

    public class LoadingPresenter : BasePresenter<LoadingView>
    {
        private IDataManager        dataManager;
        private IAddressableManager addressableManager;

        public override void OnInitialize()
        {
            this.dataManager        = ServiceProvider<IDataManager>.Get();
            this.addressableManager = ServiceProvider<IAddressableManager>.Get();
        }

        public async void StartLoading()
        {
            await this.dataManager.PopulateAllData();
            await UniTask.Delay(5000);
            await this.addressableManager.LoadScene("MainScene");
        }
    }
}