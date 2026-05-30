#nullable enable
namespace UniT
{
    using UniT.Audio.Default.DI;
    using UniT.Data.Default.DI;
    using UniT.DI.InternalDI;
    using UniT.Entities.Default.DI;
    using UniT.Lifecycle.Default.DI;
    using UniT.Logging.Unity.DI;
    using UniT.Pooling.Default.DI;
    using UniT.ResourceManagement.Addressables.DI;
    using UniT.ResourceManagement.Unity.DI;
    using UniT.UI.Default.DI;

    public static class UniTInternalDI
    {
        public static void AddUniT(this DependencyContainer container)
        {
            container.AddUnityLoggerManager();
            container.AddAddressablesAssetManager();
            container.AddAddressablesSceneManager();
            container.AddUnityExternalAssetManager();
            container.AddDataManager();
            container.AddObjectPoolManager();
            container.AddEntityManager();
            container.AddUIManager();
            container.AddAudioManager();
            container.AddLifecycleManager();
        }
    }
}