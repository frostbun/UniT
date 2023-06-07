namespace UniT.DI.Installers
{
    using UniT.Core.Addressables;
    using UniT.Core.Data.Base;
    using UniT.Core.Data.Blueprint;
    using UniT.Core.Data.Player;
    using UniT.Core.Extensions;
    using UniT.Core.Logging;
    using UniT.Core.ObjectPool;
    using Zenject;

    public class UniTInstaller : Installer<UniTInstaller>
    {
        public override void InstallBindings()
        {
            // Logging
            this.Container.BindInterfacesTo<Logger>().AsSingle();
            this.Container.Bind<LogConfig>().FromInstance(new()).AsSingle().WhenInjectedInto<ILogger>();

            // Addressables
            this.Container.BindInterfacesTo<AddressableManager>().AsSingle();

            // Object Pool
            this.Container.BindInterfacesTo<ObjectPoolManager>().AsSingle();

            // Data
            this.Container.BindInterfacesTo<DataManager>().AsSingle();
            typeof(IData).GetDerivedTypes().ForEach(type => this.Container.BindInterfacesAndSelfTo(type).AsSingle());
            this.Container.BindInterfacesTo<AddressableBlueprintJsonDataHandler>().AsSingle().WhenInjectedInto<IDataManager>();
            this.Container.BindInterfacesTo<PlayerPrefsJsonDataHandler>().AsSingle().WhenInjectedInto<IDataManager>();
        }
    }
}