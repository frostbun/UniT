#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindSerializers(this DiContainer container, IEnumerable<Type>? serializersTypes = null)
        {
            container.BindInterfacesAndSelfTo<ObjectSerializer>().AsSingle();
            #if UNIT_JSON
            container.BindInterfacesAndSelfTo<JsonSerializer>().AsSingle();
            #endif
            #if UNIT_CSV
            container.BindInterfacesAndSelfTo<CsvSerializer>().AsSingle();
            #endif

            serializersTypes?.ForEach(type =>
            {
                if (!typeof(ISerializer).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(ISerializer)}");
                container.BindInterfacesAndSelfTo(type).AsSingle();
            });
        }
    }
}
#endif