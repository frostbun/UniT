#if UNIT_ZENJECT
namespace Zenject
{
    using System.Linq;
    using UniT.Advertisements;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Data.Serializers;
    using UniT.Data.Storages;
    using UniT.Entities;
    using UniT.Logging;
    using UniT.ObjectPool;
    using UniT.ResourcesManager;
    using UniT.UI;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

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
            this.Container.BindInterfacesTo<AddressableSceneManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<AddressableAssetsManager>()
                .AsTransient()
                .Lazy();
            #else
            this.Container.BindInterfacesTo<ResourceScenesManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<ResourceAssetsManager>()
                .AsTransient()
                .Lazy();
            #endif

            #if UNIT_UNITASK
            this.Container.BindInterfacesTo<ExternalAssetsManager>()
                .AsTransient()
                .Lazy();
            #endif

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

            this.Container.BindInterfacesTo<AssetsStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<PlayerPrefsStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            #if UNIT_FBINSTANT
            this.Container.BindInterfacesTo<FbInstantStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();
            #endif

            #endregion

            this.Container.BindInterfacesTo<DataManager>()
                .AsSingle()
                .Lazy();

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