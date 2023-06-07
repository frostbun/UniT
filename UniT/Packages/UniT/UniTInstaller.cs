namespace UniT
{
    using UniT.Addressables;
    using UniT.Data.Base;
    using UniT.Data.Blueprint;
    using UniT.Data.Player;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.ObjectPool;
    using Zenject;

    public class UniTInstaller : Installer<UniTInstaller>
    {
        public override void InstallBindings()
        {
            // Logging
            this.Container.BindInterfacesTo<Logger>().AsSingle();
            this.Container.Bind<LogConfig>().FromInstance(new()).AsSingle().WhenInjectedInto<Logger>();

            // Addressables
            this.Container.BindInterfacesTo<AddressableManager>().AsSingle();

            // Object Pool
            this.Container.BindInterfacesTo<ObjectPoolManager>().AsSingle();

            // Data
            this.Container.BindInterfacesTo<DataManager>().AsSingle();
            typeof(IData).GetDerivedTypes().ForEach(type => this.Container.BindInterfacesAndSelfTo(type).AsSingle());
            this.Container.BindInterfacesTo<AddressableBlueprintJsonDataHandler>().AsSingle().WhenInjectedInto<DataManager>();
            this.Container.BindInterfacesTo<PlayerPrefsJsonDataHandler>().AsSingle().WhenInjectedInto<DataManager>();
        }
    }
}