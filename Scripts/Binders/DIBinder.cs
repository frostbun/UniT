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
    using UniT.Extensions;
    using UniT.Initializables;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourceManagement;
    using UniT.Services;
    using UniT.UI;

    public static class DIBinder
    {
        public static void AddUniT(
            this DependencyContainer container,
            RootUICanvas             rootUICanvas,
            IEnumerable<Type>?       converterTypes   = null,
            IEnumerable<Type>?       serializerTypes  = null,
            IEnumerable<Type>?       dataStorageTypes = null,
            LogLevel                 logLevel         = LogLevel.Info
        )
        {
            container.AddLoggerManager(logLevel);
            container.AddResourceManagers();
            container.AddDataManager(
                converterTypes: converterTypes,
                serializerTypes: serializerTypes,
                dataStorageTypes: dataStorageTypes
            );
            container.AddUIManager(rootUICanvas);
            container.AddObjectPoolManager();
            container.AddAudioManager();
            container.AddEntityManager();
            typeof(IService).GetDerivedTypes().ForEach(container.AddInterfacesAndSelf);
            container.AddInitializableManager();
        }
    }
}
#endif