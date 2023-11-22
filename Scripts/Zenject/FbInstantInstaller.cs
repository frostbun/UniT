#if UNIT_ZENJECT && UNIT_FBINSTANT
namespace Zenject
{
    using UniT.Advertisements;
    using UniT.Data;
    using UniT.Data.Storages;

    public sealed class FbInstantInstaller : Installer<FbInstantInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<FbInstantStorage>()
                .AsSingle()
                .WhenInjectedInto<IDataManager>()
                .Lazy();

            this.Container.BindInterfacesTo<FbInstantAdsManager>()
                .AsSingle()
                .Lazy();
        }
    }
}
#endif