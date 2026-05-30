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
    using Zenject;

    public static class UniTZenject
    {
        public static void BindUniT(this DiContainer container)
        {
            container.BindDependencyContainer();
            container.BindUnityLoggerManager();
            container.BindAddressablesAssetManager();
            container.BindAddressablesSceneManager();
            container.BindUnityExternalAssetManager();
            container.BindDataManager();
            container.BindObjectPoolManager();
            container.BindEntityManager();
            container.BindUIManager();
            container.BindAudioManager();
            container.BindLifecycleManager();
        }
    }
}