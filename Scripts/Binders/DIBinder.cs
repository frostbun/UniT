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
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourceManagement;
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
            container.AddLoggerManager(logLevel);
            container.AddResourceManagers();
            if (dataTypes is { })
            {
                container.AddDataManager(
                    dataTypes: dataTypes,
                    converterTypes: converterTypes,
                    serializerTypes: serializerTypes,
                    dataStorageTypes: dataStorageTypes
                );
            }
            if (rootUICanvas is { })
            {
                container.AddUIManager(rootUICanvas);
            }
            container.AddObjectPoolManager();
            container.AddAudioManager();
            container.AddEntityManager();
        }
    }
}
#endif