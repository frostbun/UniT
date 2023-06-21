namespace Views
{
    using Controllers;
    using UniT.Extensions;
    using UniT.UI;
    using UniT.Utils;

    public class HomeView : BaseView<HomePresenter>
    {
        public override void OnShow()
        {
            this.Presenter.StartGame();
        }

        public override void OnHide()
        {
        }
    }

    public class HomePresenter : BasePresenter<HomeView>, IInitializable
    {
        private IViewManager   viewManager;
        private GameController gameController;

        public void Initialize()
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