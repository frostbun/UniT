#if UNIT_DI
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

    public static class UniTDI
    {
        public static void AddUniT(this DependencyContainer container)
        {
            container.AddLoggerManager();
            container.AddAssetsManager();
            container.AddScenesManager();
            container.AddExternalAssetsManager();
            container.AddDataManager();
            container.AddObjectPoolManager();
            container.AddEntityManager();
            container.AddLifecycleManager();
            container.AddAudioManager();
            container.AddUIManager();
        }
    }
}
#endif