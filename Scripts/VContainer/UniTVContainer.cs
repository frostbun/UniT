#nullable enable
namespace UniT
{
    using UniT.Audio.Default.DI;
    using UniT.Data.Default.DI;
    using UniT.Entities.Default.DI;
    using UniT.Lifecycle.Default.DI;
    using UniT.Logging.Unity.DI;
    using UniT.Pooling.Default.DI;
    using UniT.ResourceManagement.Addressables.DI;
    using UniT.ResourceManagement.Unity.DI;
    using UniT.UI.Default.DI;
    using VContainer;

    public static class UniTVContainer
    {
        public static void RegisterUniT(this IContainerBuilder builder)
        {
            builder.RegisterDependencyContainer();
            builder.RegisterUnityLoggerManager();
            builder.RegisterAddressablesAssetManager();
            builder.RegisterAddressablesSceneManager();
            builder.RegisterUnityExternalAssetManager();
            builder.RegisterDataManager();
            builder.RegisterObjectPoolManager();
            builder.RegisterEntityManager();
            builder.RegisterUIManager();
            builder.RegisterAudioManager();
            builder.RegisterLifecycleManager();
        }
    }
}