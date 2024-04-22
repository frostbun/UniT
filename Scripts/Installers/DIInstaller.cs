#if UNIT_DI
namespace UniT.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.Advertisements;
    using UniT.Audio;
    using UniT.Data;
    using UniT.ECC;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.Signal;
    using UniT.UI;

    public static class DIInstaller
    {
        public static void AddUniT(this DependencyContainer container, IEnumerable<Type> dataTypes = null, RootUICanvas rootUICanvas = null, LogLevel logLevel = LogLevel.Info)
        {
            container.AddInterfaces<DIInstantiator>();

            #region Logging

            var loggerFactory = (ILoggerFactory)new LoggerFactory(() => new LogConfig { Level = logLevel });
            container.AddInterfaces(loggerFactory);
            container.AddInterfaces(loggerFactory.Create("Global"));

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            container.AddInterfaces<AddressableScenesManager>();
            container.AddInterfaces<AddressableAssetsManager>();
            #else
            container.AddInterfaces<ResourceScenesManager>();
            container.AddInterfaces<ResourceAssetsManager>();
            #endif
            container.AddInterfaces<ExternalAssetsManager>();

            #endregion

            #region Data

            if (dataTypes is { })
            {
                #region Storages

                container.AddInterfaces<AssetsNonSerializableDataStorage>();
                container.AddInterfaces<AssetsSerializableDataStorage>();
                container.AddInterfaces<PlayerPrefsDataStorage>();
                #if UNIT_FBINSTANT
                container.AddInterfaces<FbInstantDataStorage>();
                #endif

                #endregion

                #region Serializers

                container.AddInterfaces<CsvSerializer>();
                #if UNIT_NEWTONSOFT_JSON
                container.AddInterfaces<JsonSerializer>();
                #endif

                #endregion

                dataTypes.ForEach(type =>
                {
                    if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement IData");
                    container.AddInterfacesAndSelf(type);
                });
                container.AddInterfaces<DataManager>();
            }

            #endregion

            #region UI

            if (rootUICanvas is { })
            {
                container.Add(rootUICanvas);
                container.AddInterfaces<UIManager>();
            }

            #endregion

            #region Utilities

            container.AddInterfaces<AudioManager>();
            container.AddInterfaces<EntityManager>();
            container.AddInterfaces<ObjectPoolManager>();
            container.AddInterfaces<SignalBus>();

            #endregion

            #region Ads

            #if UNIT_FBINSTANT
            container.AddInterfaces<FbInstantAdsManager>();
            #else
            container.AddInterfaces<DummyAdsManager>();
            #endif

            #endregion
        }
    }
}
#endif