#if UNIT_VCONTAINER
#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using VContainer;

    public static class VContainerBinder
    {
        public static void RegisterDataStorages(this IContainerBuilder builder, IEnumerable<Type>? dataStorageTypes = null)
        {
            builder.Register<AssetDataStorage>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

            dataStorageTypes?.ForEach(type =>
            {
                if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                builder.Register(type, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            });
        }
    }
}
#endif