#if UNIT_ZENJECT
namespace UniT
{
    using System.Linq;
    using UniT.Advertisements;
    using UniT.Assets;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Data.Csv;
    using UniT.Data.Json;
    using UniT.Logging;
    using UniT.ObjectPool;
    using UniT.UI;
    using UnityEngine;
    using Zenject;
    using ILogger = UniT.Logging.ILogger;
    using SignalBus = UniT.Signal.SignalBus;

    public class UniTInstaller : Installer<UniTInstaller>
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
                    context.Container.HasBinding<LogConfig>()
                        ? context.Container.Resolve<LogConfig>()
                        : null
                ))
                .AsTransient()
                .Lazy();

            #endregion

            #region Assets

            this.Container.BindInterfacesTo<AddressableSceneManager>()
                .AsSingle()
                .Lazy();

            this.Container.BindInterfacesTo<AddressableAssetManager>()
                .AsTransient()
                .Lazy();

            this.Container.BindInterfacesTo<ExternalAssetManager>()
                .AsTransient()
                .Lazy();

            #endregion

            #region Data

            this.Container.BindInterfacesTo<PlayerPrefsJsonDataHandler>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<BlueprintAssetCsvDataHandler>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<BlueprintAssetJsonDataHandler>()
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
                    this.Container.HasBinding<IAssetManager>()
                        ? this.Container.Resolve<IAssetManager>()
                        : null,
                    this.Container.HasBinding<ILogger>()
                        ? this.Container.Resolve<ILogger>()
                        : null
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

            this.Container.BindInterfacesTo<DummyAdvertisementService>()
                .AsSingle()
                .Lazy();

            #endregion
        }
    }
}
#endif