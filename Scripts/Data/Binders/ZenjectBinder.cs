#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using UniT.Data.Conversion;
    using UniT.Data.Serialization;
    using UniT.Data.Storage;
    using UniT.Extensions;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindDataManager(
            this DiContainer   container,
            IEnumerable<Type>  dataTypes,
            IEnumerable<Type>? converterTypes   = null,
            IEnumerable<Type>? serializerTypes  = null,
            IEnumerable<Type>? dataStorageTypes = null
        )
        {
            dataTypes.ForEach(type =>
            {
                if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IData)}");
                container.BindInterfacesAndSelfTo(type).AsSingle();
            });

            container.BindConverterManager(converterTypes);
            container.BindSerializers(serializerTypes);
            container.BindDataStorages(dataStorageTypes);

            container.BindInterfacesTo<DataManager>().AsSingle();
        }
    }
}
#endif