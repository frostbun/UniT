#if UNIT_DI
#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.DI;
    using UniT.Extensions;

    public static class DIBinder
    {
        public static void AddDataManager(
            this DependencyContainer container,
            IEnumerable<Type>        dataTypes,
            IEnumerable<Type>?       converterTypes   = null,
            IEnumerable<Type>?       serializerTypes  = null,
            IEnumerable<Type>?       dataStorageTypes = null
        )
        {
            dataTypes.ForEach(type =>
            {
                if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IData)}");
                container.AddInterfacesAndSelf(type);
            });

            container.AddConverterManager(converterTypes);
            container.AddSerializers(serializerTypes);
            container.AddDataStorages(dataStorageTypes);

            container.AddInterfaces<DataManager>();
        }
    }
}
#endif