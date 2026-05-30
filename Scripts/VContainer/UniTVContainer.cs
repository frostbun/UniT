#nullable enable
namespace UniT
{
    using UniT.Audio.Default.DI;
    using UniT.Data.Converters.Default.DI;
    using UniT.Data.Default.DI;
    using UniT.Data.Serializers.Default.DI;
    using UniT.Data.Serializers.Unity.DI;
    using UniT.Data.Storages.Asset.DI;
    using UniT.Data.Storages.File.DI;
    using UniT.Data.Storages.PlayerPrefs.DI;
    using UniT.Entities.Default.DI;
    using UniT.Lifecycle.Default.DI;
    using UniT.Logging.Unity.DI;
    using UniT.Pooling.Default.DI;
    using UniT.ResourceManagement.Addressables.DI;
    using UniT.ResourceManagement.Unity.DI;
    using UniT.UI.Default.DI;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using VContainer;

    public static class UniTVContainer
    {
        public static void RegisterUniT(this IContainerBuilder builder, Canvas canvasPrefab, EventSystem eventSystemPrefab)
        {
            builder.RegisterDependencyContainer();
            builder.RegisterUnityLoggerManager();
            builder.RegisterAddressablesAssetManager();
            builder.RegisterAddressablesSceneManager();
            builder.RegisterUnityExternalAssetManager();
            builder.RegisterConverterManager();
            builder.RegisterDefaultSerializer();
            builder.RegisterUnitySerializer();
            builder.RegisterAssetStorages();
            if (Application.platform is RuntimePlatform.WebGLPlayer)
            {
                builder.RegisterPlayerPrefsStorages();
            }
            else
            {
                builder.RegisterFileStorages();
            }
            builder.RegisterDataManager();
            builder.RegisterObjectPoolManager();
            builder.RegisterEntityManager();
            builder.RegisterUIManager(canvasPrefab, eventSystemPrefab);
            builder.RegisterAudioManager();
            builder.RegisterLifecycleManager();
        }
    }
}