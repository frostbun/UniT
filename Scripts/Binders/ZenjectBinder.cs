#if UNIT_ZENJECT
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
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindUniT(
            this DiContainer   container,
            RootUICanvas?      rootUICanvas     = null,
            IEnumerable<Type>? dataTypes        = null,
            IEnumerable<Type>? converterTypes   = null,
            IEnumerable<Type>? serializerTypes  = null,
            IEnumerable<Type>? dataStorageTypes = null,
            LogLevel           logLevel         = LogLevel.Info
        )
        {
            container.BindInterfacesTo<ZenjectInstantiator>().AsSingle();

            #region Logging

            var loggerManager = (ILoggerManager)new UnityLoggerManager(logLevel);
            container.BindInterfacesTo(loggerManager.GetType()).FromInstance(loggerManager).AsSingle();
            var logger = loggerManager.GetDefaultLogger();
            container.BindInterfacesTo(logger.GetType()).FromInstance(logger).AsSingle();

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            container.BindInterfacesTo<AddressableScenesManager>().AsSingle();
            container.BindInterfacesTo<AddressableAssetsManager>().AsSingle();
            #else
            container.BindInterfacesTo<ResourceScenesManager>().AsSingle();
            container.BindInterfacesTo<ResourceAssetsManager>().AsSingle();
            #endif
            container.BindInterfacesTo<ExternalAssetsManager>().AsSingle();

            #endregion

            #region Data

            if (dataTypes is { })
            {
                container.BindDataManager(
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
                container.Bind<RootUICanvas>().FromInstance(rootUICanvas).AsSingle().WhenInjectedInto<IUIManager>();
                container.BindInterfacesTo<UIManager>().AsSingle();
            }

            #endregion

            #region Utilities

            container.BindInterfacesTo<ObjectPoolManager>().AsSingle();
            container.BindInterfacesTo<AudioManager>().AsSingle();
            container.BindInterfacesTo<EntityManager>().AsSingle();

            #endregion
        }
    }
}
#endif