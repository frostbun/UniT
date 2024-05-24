#if UNIT_DI
#nullable enable
namespace UniT.DI
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Entities;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;

    public static class DIInstaller
    {
        public static void AddUniT(
            this DependencyContainer container,
            RootUICanvas?            rootUICanvas    = null,
            IEnumerable<Type>?       dataTypes       = null,
            IEnumerable<Type>?       storageTypes    = null,
            IEnumerable<Type>?       serializerTypes = null,
            LogLevel                 logLevel        = LogLevel.Info
        )
        {
            container.AddInterfaces<DIInstantiator>();

            #region Logging

            var loggerManager = (ILoggerManager)new UnityLoggerManager(logLevel);
            container.AddInterfaces(loggerManager);
            container.AddInterfaces(loggerManager.GetDefaultLogger());

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
                #region Data

                dataTypes.ForEach(type =>
                {
                    if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IData)}");
                    container.AddInterfacesAndSelf(type);
                });

                #endregion

                #region Storages

                container.AddInterfaces<AssetDataStorage>();
                container.AddInterfaces<PlayerPrefsDataStorage>();
                storageTypes?.ForEach(type =>
                {
                    if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                    container.AddInterfaces(type);
                });

                #endregion

                #region Serializers

                container.AddInterfaces<CsvSerializer>();
                #if UNIT_NEWTONSOFT_JSON
                container.AddInterfaces<JsonSerializer>();
                #endif
                serializerTypes?.ForEach(type =>
                {
                    if (!typeof(ISerializer).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(ISerializer)}");
                    container.AddInterfaces(type);
                });

                #endregion

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

            container.AddInterfaces<ObjectPoolManager>();
            container.AddInterfaces<AudioManager>();
            container.AddInterfaces<EntityManager>();

            #endregion
        }
    }
}
#endif