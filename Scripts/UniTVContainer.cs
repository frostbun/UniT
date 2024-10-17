#if UNIT_VCONTAINER
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
    using VContainer;

    public static class UniTVContainer
    {
        public static void RegisterUniT(this IContainerBuilder builder)
        {
            builder.RegisterDependencyContainer();
            builder.RegisterLoggerManager();
            builder.RegisterAssetsManager();
            builder.RegisterDataManager();
            builder.RegisterUIManager();
            builder.RegisterObjectPoolManager();
            builder.RegisterAudioManager();
            builder.RegisterEntityManager();
            builder.RegisterInitializableManager();
        }
    }
}
#endif