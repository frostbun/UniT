namespace Views
{
    using Controllers;
    using UniT.Extensions;
    using UniT.UI;
    using UniT.Utils;

    public class HomeView : BaseView<HomePresenter>
    {
        protected override void OnShow()
        {
            this.Presenter.StartGame();
        }
    }

    public class HomePresenter : BasePresenter<HomeView>
    {
        private IViewManager   viewManager;
        private GameController gameController;

        protected override void Initialize()
        {
            this.viewManager    = ServiceProvider<IViewManager>.Get();
            this.gameController = ServiceProvider<GameController>.Get();
        }

        public void StartGame()
        {
            this.gameController.Initialize();
        }
    }
}