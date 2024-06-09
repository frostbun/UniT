#if UNIT_ZENJECT
#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using Zenject;

    public static class ZenjectBinder
    {
        public static void BindDataStorages(this DiContainer container, IEnumerable<Type>? dataStorageTypes = null)
        {
            container.BindInterfacesAndSelfTo<AssetDataStorage>().AsSingle();
            container.BindInterfacesAndSelfTo<PlayerPrefsDataStorage>().AsSingle();

            dataStorageTypes?.ForEach(type =>
            {
                if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                container.BindInterfacesAndSelfTo(type).AsSingle();
            });
        }
    }
}
#endif