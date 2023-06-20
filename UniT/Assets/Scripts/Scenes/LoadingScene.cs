namespace Scenes
{
    using BlueprintData;
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.Data.Csv.Blueprint;
    using UniT.Data.Json.Player;
    using UniT.Logging;
    using UniT.ObjectPool;
    using UniT.UI;
    using UniT.Utils;
    using UnityEngine;
    using Views;
    using ILogger = UniT.Logging.ILogger;
    using Logger = UniT.Logging.Logger;

    public class LoadingScene : MonoBehaviour
    {
        [SerializeField]
        private ViewManager viewManager;

        [SerializeField]
        private LoadingView loadingView;

        private void Awake()
        {
            #region ServiceProvider

            ServiceProvider<ILogger>.Add(new Logger(new LogConfig(LogLevel.Debug)));

            ServiceProvider<IAddressableManager>.Add(new AddressableManager(ServiceProvider<ILogger>.Get()));

            ServiceProvider<IObjectPoolManager>.Add(
                new ObjectPoolManager(
                    ServiceProvider<IAddressableManager>.Get(),
                    ServiceProvider<ILogger>.Get()
                )
            );

            ServiceProvider<LevelBlueprint>.Add(new LevelBlueprint());
            ServiceProvider<IData>.Add(ServiceProvider<LevelBlueprint>.Get());
            ServiceProvider<IDataHandler>.Add(
                new PlayerPrefsJsonDataHandler(),
                new BlueprintAddressableCsvDataHandler(ServiceProvider<IAddressableManager>.Get())
            );
            ServiceProvider<IDataManager>.Add(
                new DataManager(
                    ServiceProvider<IData>.GetAll(),
                    ServiceProvider<IDataHandler>.GetAll(),
                    ServiceProvider<ILogger>.Get()
                )
            );

            DontDestroyOnLoad(this.viewManager);
            this.viewManager.Inject(ServiceProvider<IAddressableManager>.Get(), ServiceProvider<ILogger>.Get());
            ServiceProvider<IViewManager>.Add(this.viewManager);

            #endregion
        }

        private void Start()
        {
            this.viewManager.GetView<LoadingView, LoadingPresenter>(this.loadingView).Stack();
        }
    }
}