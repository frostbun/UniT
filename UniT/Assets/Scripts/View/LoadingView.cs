namespace View
{
    using BlueprintData;
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.Data.Csv.Blueprint;
    using UniT.Data.Json.Player;
    using UniT.Logging;
    using UniT.ObjectPool;
    using UniT.Utils;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;
    using Logger = UniT.Logging.Logger;

    public class LoadingView : MonoBehaviour
    {
        private void Awake()
        {
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
        }

        private async void Start()
        {
            await ServiceProvider<IDataManager>.Get().PopulateAllData();
            var levelBlueprint = ServiceProvider<LevelBlueprint>.Get();
        }
    }
}