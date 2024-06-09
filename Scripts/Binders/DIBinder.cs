#if UNIT_DI
#nullable enable
namespace UniT
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.DI;
    using UniT.Entities;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;

    public static class DIBinder
    {
        public static void AddUniT(
            this DependencyContainer container,
            RootUICanvas?            rootUICanvas     = null,
            IEnumerable<Type>?       dataTypes        = null,
            IEnumerable<Type>?       converterTypes   = null,
            IEnumerable<Type>?       serializerTypes  = null,
            IEnumerable<Type>?       dataStorageTypes = null,
            LogLevel                 logLevel         = LogLevel.Info
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
                container.AddDataManager(
                    dataTypes: dataTypes,
                    converterTypes: converterTypes,
                    serializerTypes: serializerTypes,
                    dataStorageTypes: dataStorageTypes
                );
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