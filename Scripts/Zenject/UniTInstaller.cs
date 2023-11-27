#if UNIT_ZENJECT
namespace Zenject
{
    using System.Linq;
    using UniT.Advertisements;
    using UniT.Assets;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Data.Serializers;
    using UniT.Data.Storages;
    using UniT.Logging;
    using UniT.ObjectPool;
    using UniT.UI;
    using UnityEngine;
    using ILogger = UniT.Logging.ILogger;

    public sealed class UniTInstaller : Installer<UniTInstaller>
    {
        public override void InstallBindings()
        {
            #region Logging

            this.Container.Bind<LogConfig>()
                .FromMethod(_ => new()
                {
                    Level = LogLevel.Info,
                })
                .AsTransient()
                .WhenInjectedInto<ILogger>()
                .Lazy();

            this.Container.BindInterfacesTo<UnityLogger>()
                .FromMethod(context => new UnityLogger(
                    context.ObjectType?.Name,
                    context.Container.TryResolve<LogConfig>()
                ))
                .AsTransient()
                .Lazy();

            #endregion

            #region Assets

            this.Container.BindInterfacesTo<AddressableSceneManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<AddressableAssetsManager>()
                .AsTransient()
                .Lazy();

            #if UNIT_UNITASK
            this.Container.BindInterfacesTo<ExternalAssetsManager>()
                .AsTransient()
                .Lazy();
            #endif

            #endregion

            #region Data

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

            this.Container.BindInterfacesTo<AssetsStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<PlayerPrefsStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<DataManager>()
                .AsSingle()
                .Lazy();

            #endregion

            #region Utilities

            this.Container.BindInterfacesTo<UIManager>()
                .FromMethod(_ => Object.FindObjectsOfType<UIManager>().Single().Construct(
                    new(type => (IPresenter)CurrentContext.Container.Instantiate(type)),
                    this.Container.TryResolve<IAssetsManager>(),
                    this.Container.TryResolve<ILogger>()
                ))
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<AudioManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<ObjectPoolManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<SignalBus>()
                .AsSingle()
                .Lazy();

            #endregion

            #region Third Party

            this.Container.BindInterfacesTo<DummyAdsManager>()
                .AsSingle()
                .Lazy();

            #endregion
        }
    }
}
#endif