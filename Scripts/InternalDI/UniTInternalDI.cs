#nullable enable
namespace UniT
{
    using UniT.Audio.Default.DI;
    using UniT.Data.Converters.Default.DI;
    using UniT.Data.Default.DI;
    using UniT.Data.Serializers.Default.DI;
    using UniT.Data.Serializers.Unity.DI;
    using UniT.Data.Storages.Asset.DI;
    using UniT.DI;
    using UniT.Entities.Default.DI;
    using UniT.Lifecycle.Default.DI;
    using UniT.Logging.Unity.DI;
    using UniT.Pooling.Default.DI;
    using UniT.ResourceManagement.Addressables.DI;
    using UniT.ResourceManagement.Unity.DI;
    using UniT.UI.Default.DI;
    using UnityEngine;
    using UnityEngine.EventSystems;
    #if !UNITY_WEBGL
    using UniT.Data.Storages.File.DI;
    #else
    using UniT.Data.Storages.PlayerPrefs.DI;
    #endif

    public static class UniTInternalDI
    {
        public static void AddUniT(this DependencyContainer container, Canvas canvasPrefab, EventSystem eventSystemPrefab)
        {
            container.AddUnityLoggerManager();
            container.AddAddressablesAssetManager();
            container.AddAddressablesSceneManager();
            container.AddUnityExternalAssetManager();
            container.AddConverterManager();
            container.AddDefaultSerializer();
            container.AddUnitySerializer();
            container.AddAssetStorages();
            #if !UNITY_WEBGL
            container.AddFileStorages();
            #else
            container.AddPlayerPrefsStorages();
            #endif
            container.AddDataManager();
            container.AddObjectPoolManager();
            container.AddEntityManager();
            container.AddUIManager(canvasPrefab, eventSystemPrefab);
            container.AddAudioManager();
            container.AddLifecycleManager();
        }
    }
}