namespace UniT.Data.Base
{
    using UniT.Data.Blueprint;
    using UniT.Data.Player;
    using UniT.Extensions;
    using Zenject;

    public class DataInstaller : Installer<DataInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.Bind<DataManager>().AsSingle();
            typeof(IData).GetDerivedTypes().ForEach(type => this.Container.BindInterfacesAndSelfTo(type).AsSingle());
            this.Container.BindInterfacesTo<LocalBlueprintJsonDataHandler>().AsSingle().WhenInjectedInto<DataManager>();
            this.Container.BindInterfacesTo<LocalPlayerJsonDataHandler>().AsSingle().WhenInjectedInto<DataManager>();
        }
    }
}