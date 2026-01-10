#if UNIT_VCONTAINER
#nullable enable
namespace UniT
{
    using UniT.Audio.DI;
    using UniT.Data.DI;
    using UniT.DI;
    using UniT.Entities.DI;
    using UniT.Lifecycle.DI;
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
            builder.RegisterScenesManager();
            builder.RegisterExternalAssetsManager();
            builder.RegisterDataManager();
            builder.RegisterObjectPoolManager();
            builder.RegisterEntityManager();
            builder.RegisterLifecycleManager();
            builder.RegisterAudioManager();
            builder.RegisterUIManager();
        }
    }
}
#endif