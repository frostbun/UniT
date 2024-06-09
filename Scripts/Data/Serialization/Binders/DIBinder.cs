#if UNIT_DI
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Collections.Generic;
    using UniT.DI;
    using UniT.Extensions;

    public static class DIBinder
    {
        public static void AddSerializers(this DependencyContainer container, IEnumerable<Type>? serializersTypes = null)
        {
            container.AddInterfacesAndSelf<ObjectSerializer>();
            #if UNIT_JSON
            container.AddInterfacesAndSelf<JsonSerializer>();
            #endif
            #if UNIT_CSV
            container.AddInterfacesAndSelf<CsvSerializer>();
            #endif

            serializersTypes?.ForEach(type =>
            {
                if (!typeof(ISerializer).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(ISerializer)}");
                container.AddInterfacesAndSelf(type);
            });
        }
    }
}
#endif