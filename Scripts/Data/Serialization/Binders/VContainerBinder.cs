#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using VContainer;

    public static class VContainerBinder
    {
        public static void RegisterSerializers(this IContainerBuilder builder, IEnumerable<Type>? serializersTypes = null)
        {
            builder.Register<ObjectSerializer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #if UNIT_JSON
            builder.Register<JsonSerializer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif
            #if UNIT_CSV
            builder.Register<CsvSerializer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            #endif

            serializersTypes?.ForEach(type =>
            {
                if (!typeof(ISerializer).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(ISerializer)}");
                builder.Register(type, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            });
        }
    }
}
#endif