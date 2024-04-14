#if UNIT_ZENJECT
namespace Zenject
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

    public static class ZenjectInstaller
    {
        public static void BindUniT(this DiContainer container, RootUICanvas rootUICanvas = null, params Type[] dataTypes)
        {
            container.BindInterfacesTo<LoggerFactory>().AsSingle().WhenInjectedInto<IHasLogger>();
            container.BindInterfacesTo<ZenjectInstantiator>().AsSingle();

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

            #region Storages

            container.BindInterfacesTo<AssetsNonSerializableDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
            container.BindInterfacesTo<AssetsSerializableDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
            container.BindInterfacesTo<PlayerPrefsDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
            #if UNIT_FBINSTANT
            container.BindInterfacesTo<FbInstantDataStorage>().AsSingle().WhenInjectedInto<IDataManager>();
            #endif

            #endregion

            #region Serializers

            #if UNIT_NEWTONSOFT_JSON
            container.BindInterfacesTo<JsonSerializer>().AsSingle().WhenInjectedInto<IDataManager>();
            #endif
            container.BindInterfacesTo<CsvSerializer>().AsSingle().WhenInjectedInto<IDataManager>();

            #endregion

            dataTypes.ForEach(type => container.BindInterfacesAndSelfTo(type).AsSingle());
            container.BindInterfacesTo<DataManager>().AsSingle();

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
            container.BindInterfacesTo<EntityManager>().AsSingle();
            container.BindInterfacesTo<ObjectPoolManager>().AsSingle();
            container.BindInterfacesTo<SignalBus>().AsSingle();

            #endregion

            #region Ads

            #if UNIT_FBINSTANT
            container.BindInterfacesTo<FbInstantAdsManager>().AsSingle();
            #else
            container.BindInterfacesTo<DummyAdsManager>().AsSingle();
            #endif

            #endregion
        }
    }
}
#endif