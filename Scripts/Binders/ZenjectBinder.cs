#if UNIT_ZENJECT
#nullable enable
namespace UniT
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.DI;
    using UniT.Entities;
    using UniT.Extensions;
    using UniT.Initializables;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourceManagement;
    using UniT.Services;
    using UniT.UI;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindUniT(
            this DiContainer   container,
            RootUICanvas       rootUICanvas,
            IEnumerable<Type>? converterTypes   = null,
            IEnumerable<Type>? serializerTypes  = null,
            IEnumerable<Type>? dataStorageTypes = null,
            LogLevel           logLevel         = LogLevel.Info
        )
        {
            container.BindInterfacesTo<ZenjectContainer>().AsSingle();
            container.BindLoggerManager(logLevel);
            container.BindResourceManagers();
            container.BindDataManager(
                converterTypes: converterTypes,
                serializerTypes: serializerTypes,
                dataStorageTypes: dataStorageTypes
            );
            container.BindUIManager(rootUICanvas);
            container.BindObjectPoolManager();
            container.BindAudioManager();
            container.BindEntityManager();
            typeof(IService).GetDerivedTypes().ForEach(type => container.BindInterfacesAndSelfTo(type).AsSingle());
            container.BindInitializableManager();
        }
    }
}
#endif