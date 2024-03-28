#if UNIT_DI
namespace UniT.DI
{
    using System;
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
        public static void AddUniT(this DependencyContainer container, RootUICanvas rootUICanvas, params Type[] dataTypes)
        {
            container.AddInterfaces<LoggerFactory>();
            container.AddInterfaces<DIInstantiator>();

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

            #region Storages

            container.AddInterfaces<AssetsNonSerializableDataStorage>();
            container.AddInterfaces<AssetsSerializableDataStorage>();
            container.AddInterfaces<PlayerPrefsDataStorage>();
            #if UNIT_FBINSTANT
            container.AddInterfaces<FbInstantDataStorage>();
            #endif

            #endregion

            #region Serializers

            #if UNIT_NEWTONSOFT_JSON
            container.AddInterfaces<JsonSerializer>();
            #endif
            container.AddInterfaces<CsvSerializer>();

            #endregion

            dataTypes.ForEach(container.AddInterfacesAndSelf);
            container.AddInterfaces<DataManager>();

            #endregion

            #region UI

            container.Add(rootUICanvas);
            container.AddInterfaces<UIManager>();

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