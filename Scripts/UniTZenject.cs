#if UNIT_ZENJECT
#nullable enable
namespace UniT
{
    using UniT.Audio.DI;
    using UniT.Data.DI;
    using UniT.DI;
    using UniT.Entities.DI;
    using UniT.Initializables.DI;
    using UniT.Logging.DI;
    using UniT.Pooling.DI;
    using UniT.ResourceManagement.DI;
    using UniT.UI.DI;
    using Zenject;

    public static class UniTZenject
    {
        public static void BindUniT(this DiContainer container)
        {
            container.BindDependencyContainer();
            container.BindLoggerManager();
            container.BindAssetsManager();
            container.BindDataManager();
            container.BindUIManager();
            container.BindObjectPoolManager();
            container.BindAudioManager();
            container.BindEntityManager();
            container.BindInitializableManager();
        }
    }
}
#endif