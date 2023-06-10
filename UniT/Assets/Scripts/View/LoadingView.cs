namespace View
{
    using UniT.Core.Addressables;
    using UniT.Core.Data.Base;
    using UniT.Core.Data.Blueprint;
    using UniT.Core.Data.Player;
    using UniT.Core.Logging;
    using UniT.Core.ObjectPool;
    using UniT.Core.Utils;
    using UnityEngine;
    using ILogger = UniT.Core.Logging.ILogger;
    using Logger = UniT.Core.Logging.Logger;

    public class LoadingView : MonoBehaviour
    {
        private void Awake()
        {
            ServiceProvider<ILogger>.Add(new Logger(new LogConfig()));

            ServiceProvider<IAddressableManager>.Add(new AddressableManager(ServiceProvider<ILogger>.Get()));

            ServiceProvider<IObjectPoolManager>.Add(
                new ObjectPoolManager(
                    ServiceProvider<IAddressableManager>.Get(),
                    ServiceProvider<ILogger>.Get()
                )
            );

            ServiceProvider<IData>.Add();
            ServiceProvider<IDataHandler>.Add(
                new PlayerPrefsJsonDataHandler(),
                new AddressableBlueprintJsonDataHandler(ServiceProvider<IAddressableManager>.Get())
            );
            ServiceProvider<IDataManager>.Add(
                new DataManager(
                    ServiceProvider<IData>.GetAll(),
                    ServiceProvider<IDataHandler>.GetAll(),
                    ServiceProvider<ILogger>.Get()
                )
            );
        }
    }
}