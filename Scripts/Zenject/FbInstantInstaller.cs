#if UNIT_ZENJECT && UNIT_FBINSTANT
namespace UniT
{
    using UniT.Advertisements;
    using UniT.Data.Json;
    using Zenject;

    public sealed class FbInstantInstaller : Installer<FbInstantInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<FbInstantPlayerJsonDataHandler>()
                .AsSingle()
                .Lazy();

            this.Container.UnbindInterfacesTo<DummyAdvertisementService>();
            this.Container.BindInterfacesTo<FbInstantAdvertisementService>()
                .AsSingle()
                .Lazy();
        }
    }
}
#endif