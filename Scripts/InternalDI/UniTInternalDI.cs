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
            if (Application.platform is RuntimePlatform.WebGLPlayer)
            {
                container.AddPlayerPrefsStorages();
            }
            else
            {
                container.AddFileStorages();
            }
            container.AddDataManager();
            container.AddObjectPoolManager();
            container.AddEntityManager();
            container.AddUIManager(canvasPrefab, eventSystemPrefab);
            container.AddAudioManager();
            container.AddLifecycleManager();
        }
    }
}