#if UNIT_ZENJECT
#nullable enable
namespace UniT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Audio;
    using UniT.Data;
    using UniT.DI;
    using UniT.Entities;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.Models;
    using UniT.Pooling;
    using UniT.ResourceManagement;
    using UniT.Services;
    using UniT.UI;
    using Zenject;
    using InitializableManager = UniT.Services.InitializableManager;

    public static class ZenjectBinder
    {
        public static void BindUniT(
            this DiContainer   container,
            RootUICanvas       rootUICanvas,
            IEnumerable<Type>? dataTypes        = null,
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
                dataTypes: typeof(IConfig).GetDerivedTypes().Concat(typeof(IProgression).GetDerivedTypes()).Concat(dataTypes ?? Enumerable.Empty<Type>()),
                converterTypes: converterTypes,
                serializerTypes: serializerTypes,
                dataStorageTypes: dataStorageTypes
            );
            container.BindUIManager(rootUICanvas);
            container.BindObjectPoolManager();
            container.BindAudioManager();
            container.BindEntityManager();
            typeof(IService).GetDerivedTypes().ForEach(type => container.BindInterfacesAndSelfTo(type).AsSingle());
            container.Bind<InitializableManager>().AsSingle();
        }
    }
}
#endif