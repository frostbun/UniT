using BlueprintData;
using Controllers;
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
using Logger = UniT.Logging.Logger;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private ViewManager viewManager;

    [SerializeField]
    private LoadingView loadingView;

    [SerializeField]
    private GameController gameController;

    private void Awake()
    {
        #region ServiceProvider

        var addressableManager = new AddressableManager(new Logger(new(LogLevel.Info)));
        ServiceProvider<IAddressableManager>.Add(addressableManager);

        var objectPoolManager = new ObjectPoolManager(addressableManager, new Logger(new(LogLevel.Info)));
        ServiceProvider<IObjectPoolManager>.Add(objectPoolManager);

        var levelBlueprint = new LevelBlueprint();
        ServiceProvider<LevelBlueprint>.Add(levelBlueprint);

        var dataManager = new DataManager(
            new IData[] { levelBlueprint },
            new IDataHandler[]
            {
                new PlayerPrefsJsonDataHandler(),
                new BlueprintAddressableCsvDataHandler(addressableManager),
            },
            new Logger(new(LogLevel.Info))
        );
        ServiceProvider<IDataManager>.Add(dataManager);

        DontDestroyOnLoad(this.viewManager);
        this.viewManager.Inject(addressableManager, new Logger(new(LogLevel.Debug)));
        ServiceProvider<IViewManager>.Add(this.viewManager);

        DontDestroyOnLoad(this.gameController);
        ServiceProvider<GameController>.Add(this.gameController);

        #endregion
    }

    private void Start()
    {
        this.viewManager.GetView<LoadingView, LoadingPresenter>(this.loadingView).Stack();
        Destroy(this.gameObject);
    }
}