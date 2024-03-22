#if UNIT_ZENJECT
namespace Zenject
{
    using UniT.Advertisements;
    using UniT.Audio;
    using UniT.Data;
    using UniT.ECC;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;

    public sealed class UniTInstaller : Installer<UniTInstaller>
    {
        public override void InstallBindings()
        {
            #region Logging

            this.Container.BindInterfacesTo<LoggerFactory>()
                .AsSingle()
                .WhenInjectedInto<IHasLogger>()
                .Lazy();

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            this.Container.BindInterfacesTo<AddressableScenesManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<AddressableAssetsManager>()
                .AsSingle()
                .Lazy();
            #else
            this.Container.BindInterfacesTo<ResourceScenesManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<ResourceAssetsManager>()
                .AsSingle()
                .Lazy();
            #endif

            this.Container.BindInterfacesTo<ExternalAssetsManager>()
                .AsSingle()
                .Lazy();

            #endregion

            #region Data

            #region Serializers

            #if UNIT_NEWTONSOFT_JSON
            this.Container.BindInterfacesTo<JsonSerializer>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();
            #endif

            this.Container.BindInterfacesTo<CsvSerializer>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            #endregion

            #region Storages

            this.Container.BindInterfacesTo<AssetsNonSerializableDataStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<AssetsSerializableDataStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<PlayerPrefsDataStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            #if UNIT_FBINSTANT
            this.Container.BindInterfacesTo<FbInstantDataStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();
            #endif

            #endregion

            this.Container.BindInterfacesTo<DataManager>()
                .AsSingle()
                .Lazy();

            #endregion

            #region Instantiator

            #if UNIT_ZENJECT
            this.Container.BindInterfacesTo<ZenjectInstantiator>()
                .AsSingle()
                .Lazy();
            #elif UNIT_DI
            this.Container.BindInterfacesTo<DIInstantiator>()
                .AsSingle()
                .Lazy();
            #endif

            #endregion

            #region Utilities

            // this.Container.BindInterfacesTo<UIManager>()
            //     .FromMethod(_ => Object.FindObjectsOfType<UIManager>().Single().Construct(
            //         new(type => (IPresenter)CurrentContext.Container.Instantiate(type)),
            //         this.Container.TryResolve<IAssetsManager>(),
            //         this.Container.TryResolve<ILogger>()
            //     ))
            //     .AsSingle()
            //     .Lazy();

            this.Container.BindInterfacesTo<AudioManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<EntityManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<ObjectPoolManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<SignalBus>()
                .AsSingle()
                .Lazy();

            #endregion

            #region Ads

            #if UNIT_FBINSTANT
            this.Container.BindInterfacesTo<FbInstantAdsManager>()
                .AsSingle()
                .Lazy();
            #else
            this.Container.BindInterfacesTo<DummyAdsManager>()
                .AsSingle()
                .Lazy();
            #endif

            #endregion
        }
    }
}
#endif