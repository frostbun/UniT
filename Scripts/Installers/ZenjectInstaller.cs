#if UNIT_ZENJECT
namespace Zenject
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.ECC;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;

    public static class ZenjectInstaller
    {
        public static void BindUniT(this DiContainer container, IEnumerable<Type> dataTypes = null, RootUICanvas rootUICanvas = null, LogLevel logLevel = LogLevel.Info)
        {
            container.BindInterfacesTo<ZenjectInstantiator>().AsSingle();

            #region Logging

            var loggerFactory = (ILoggerManager)new UnityLoggerManager(logLevel);
            container.BindInterfacesTo(loggerFactory.GetType()).FromInstance(loggerFactory).AsSingle();
            var logger = loggerFactory.GetDefaultLogger();
            container.BindInterfacesTo(logger.GetType()).FromInstance(logger).AsSingle();

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            container.BindInterfacesTo<AddressableScenesManager>().AsSingle();
            container.BindInterfacesTo<AddressableAssetsManager>().AsSingle();
            #else
            container.BindInterfacesTo<ResourceScenesManager>().AsSingle();
            container.BindInterfacesTo<ResourceAssetsManager>().AsSingle();
            #endif
            container.BindInterfacesTo<ExternalAssetsManager>().AsSingle();

            #endregion

            #region Data

            if (dataTypes is { })
            {
                #region Storages

                container.BindInterfacesTo<AssetDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
                container.BindInterfacesTo<PlayerPrefsDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
                #if UNIT_FBINSTANT
                container.BindInterfacesTo<FbInstantDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
                #endif

                #endregion

                #region Serializers

                container.BindInterfacesTo<CsvSerializer>().AsSingle().WhenInjectedInto<IDataManager>();
                #if UNIT_NEWTONSOFT_JSON
                container.BindInterfacesTo<JsonSerializer>().AsSingle().WhenInjectedInto<IDataManager>();
                #endif

                #endregion

                dataTypes.ForEach(type =>
                {
                    if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement IData");
                    container.BindInterfacesAndSelfTo(type).AsSingle();
                });
                container.BindInterfacesTo<DataManager>().AsSingle();
            }

            #endregion

            #region UI

            if (rootUICanvas is { })
            {
                container.Bind<RootUICanvas>().FromInstance(rootUICanvas).AsSingle().WhenInjectedInto<IUIManager>();
                container.BindInterfacesTo<UIManager>().AsSingle();
            }

            #endregion

            #region Utilities

            container.BindInterfacesTo<AudioManager>().AsSingle();
            container.BindInterfacesTo<ObjectPoolManager>().AsSingle();
            container.BindInterfacesTo<EntityManager>().AsSingle();

            #endregion
        }
    }
}
#endif