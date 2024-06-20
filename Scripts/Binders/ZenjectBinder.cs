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
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourceManagement;
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
            container.BindInterfacesTo<ZenjectContainer>().AsSingle().CopyIntoAllSubContainers();
            container.BindLoggerManager(logLevel);
            container.BindResourceManagers();
            if (dataTypes is { })
            {
                container.BindDataManager(
                    dataTypes: dataTypes,
                    converterTypes: converterTypes,
                    serializerTypes: serializerTypes,
                    dataStorageTypes: dataStorageTypes
                );
            }
            if (rootUICanvas is { })
            {
                container.BindUIManager(rootUICanvas);
            }
            container.BindObjectPoolManager();
            container.BindAudioManager();
            container.BindEntityManager();
        }
    }
}
#endif