#if UNIT_DI
#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using UniT.DI;
    using UniT.Extensions;

    public static class DIBinder
    {
        public static void AddDataStorages(this DependencyContainer container, IEnumerable<Type>? dataStorageTypes = null)
        {
            container.AddInterfacesAndSelf<AssetDataStorage>();
            container.AddInterfacesAndSelf<PlayerPrefsDataStorage>();

            dataStorageTypes?.ForEach(type =>
            {
                if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                container.AddInterfacesAndSelf(type);
            });
        }
    }
}
#endif