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
    using Zenject;

    public static class UniTZenject
    {
        public static void BindUniT(this DiContainer container, Canvas canvasPrefab, EventSystem eventSystemPrefab)
        {
            container.BindDependencyContainer();
            container.BindUnityLoggerManager();
            container.BindAddressablesAssetManager();
            container.BindAddressablesSceneManager();
            container.BindUnityExternalAssetManager();
            container.BindConverterManager();
            container.BindDefaultSerializer();
            container.BindUnitySerializer();
            container.BindAssetStorages();
            if (Application.platform is RuntimePlatform.WebGLPlayer)
            {
                container.BindPlayerPrefsStorages();
            }
            else
            {
                container.BindFileStorages();
            }
            container.BindDataManager();
            container.BindObjectPoolManager();
            container.BindEntityManager();
            container.BindUIManager(canvasPrefab, eventSystemPrefab);
            container.BindAudioManager();
            container.BindLifecycleManager();
        }
    }
}