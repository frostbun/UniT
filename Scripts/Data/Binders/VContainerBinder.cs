#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.Extensions;
    using VContainer;

    public static class VContainerBinder
    {
        public static void RegisterDataManager(
            this IContainerBuilder builder,
            IEnumerable<Type>      dataTypes,
            IEnumerable<Type>?     converterTypes   = null,
            IEnumerable<Type>?     serializerTypes  = null,
            IEnumerable<Type>?     dataStorageTypes = null
        )
        {
            dataTypes.ForEach(type =>
            {
                if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IData)}");
                builder.Register(type, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            });

            builder.RegisterConverterManager(converterTypes);
            builder.RegisterSerializers(serializerTypes);
            builder.RegisterDataStorages(dataStorageTypes);

            builder.Register<DataManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
#endif