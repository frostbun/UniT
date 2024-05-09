#if UNIT_DI
namespace UniT.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.ECC;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;

    public static class DIInstaller
    {
        public static void AddUniT(this DependencyContainer container, IEnumerable<Type> dataTypes = null, RootUICanvas rootUICanvas = null, LogLevel logLevel = LogLevel.Info)
        {
            container.AddInterfaces<DIInstantiator>();

            #region Logging

            var loggerFactory = (ILoggerManager)new UnityLoggerManager(logLevel);
            container.AddInterfaces(loggerFactory);
            container.AddInterfaces(loggerFactory.GetDefaultLogger());

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            container.AddInterfaces<AddressableScenesManager>();
            container.AddInterfaces<AddressableAssetsManager>();
            #else
            container.AddInterfaces<ResourceScenesManager>();
            container.AddInterfaces<ResourceAssetsManager>();
            #endif
            container.AddInterfaces<ExternalAssetsManager>();

            #endregion

            #region Data

            if (dataTypes is { })
            {
                #region Storages

                container.AddInterfaces<AssetDataStorage>();
                container.AddInterfaces<PlayerPrefsDataStorage>();
                #if UNIT_FBINSTANT && UNITY_WEBGL && !UNITY_EDITOR
                container.AddInterfaces<FbInstantDataStorage>();
                #endif

                #endregion

                #region Serializers

                container.AddInterfaces<CsvSerializer>();
                #if UNIT_NEWTONSOFT_JSON
                container.AddInterfaces<JsonSerializer>();
                #endif

                #endregion

                dataTypes.ForEach(type =>
                {
                    if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement IData");
                    container.AddInterfacesAndSelf(type);
                });
                container.AddInterfaces<DataManager>();
            }

            #endregion

            #region UI

            if (rootUICanvas is { })
            {
                container.Add(rootUICanvas);
                container.AddInterfaces<UIManager>();
            }

            #endregion

            #region Utilities

            container.AddInterfaces<AudioManager>();
            container.AddInterfaces<ObjectPoolManager>();
            container.AddInterfaces<EntityManager>();

            #endregion
        }
    }
}
#endif