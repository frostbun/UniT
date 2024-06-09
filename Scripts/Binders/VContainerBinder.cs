#if UNIT_VCONTAINER
#nullable enable
namespace UniT
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Entities;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;
    using VContainer;

    public static class VContainerBinder
    {
        public static void RegisterUniT(
            this IContainerBuilder builder,
            RootUICanvas?          rootUICanvas     = null,
            IEnumerable<Type>?     dataTypes        = null,
            IEnumerable<Type>?     converterTypes   = null,
            IEnumerable<Type>?     serializerTypes  = null,
            IEnumerable<Type>?     dataStorageTypes = null,
            LogLevel               logLevel         = LogLevel.Info
        )
        {
            builder.Register<VContainerInstantiator>(Lifetime.Singleton).AsImplementedInterfaces();

            #region Logging

            var loggerManager = (ILoggerManager)new UnityLoggerManager(logLevel);
            builder.RegisterInstance(loggerManager).AsImplementedInterfaces();
            builder.RegisterInstance(loggerManager.GetDefaultLogger()).AsImplementedInterfaces();

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            builder.Register<AddressableScenesManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AddressableAssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            #else
            builder.Register<ResourceScenesManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ResourceAssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
            builder.Register<ExternalAssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion

            #region Data

            if (dataTypes is { })
            {
                builder.RegisterDataManager(
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
                builder.RegisterInstance(rootUICanvas).AsSelf();
                builder.Register<UIManager>(Lifetime.Singleton).AsImplementedInterfaces();
            }

            #endregion

            #region Utilities

            builder.Register<ObjectPoolManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AudioManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityManager>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion
        }
    }
}
#endif