#if UNIT_VCONTAINER
namespace VContainer
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.Entities;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;

    public static class VContainerInstaller
    {
        public static void RegisterUniT(
            this IContainerBuilder builder,
            RootUICanvas           rootUICanvas    = null,
            IEnumerable<Type>      dataTypes       = null,
            IEnumerable<Type>      storageTypes    = null,
            IEnumerable<Type>      serializerTypes = null,
            LogLevel               logLevel        = LogLevel.Info
        )
        {
            builder.Register<VContainerInstantiator>(Lifetime.Singleton).AsImplementedInterfaces();

            #region Logging

            var loggerFactory = (ILoggerManager)new UnityLoggerManager(logLevel);
            builder.RegisterInstance(loggerFactory).AsImplementedInterfaces();
            builder.RegisterInstance(loggerFactory.GetDefaultLogger()).AsImplementedInterfaces();

            #endregion

            #region ResourcesManager

            #if UNIT_ADDRESSABLES
            builder.Register<AddressableScenesManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AddressableAssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            #else
            container.Register<ResourceScenesManager>(Lifetime.Singleton).AsImplementedInterfaces();
            container.Register<ResourceAssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();
            #endif
            builder.Register<ExternalAssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion

            #region Data

            if (dataTypes is { })
            {
                #region Data

                dataTypes.ForEach(type =>
                {
                    if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IData)}");
                    builder.Register(type, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
                });

                #endregion

                #region Storages

                builder.Register<AssetDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
                builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
                storageTypes?.ForEach(type =>
                {
                    if (!typeof(IDataStorage).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(IDataStorage)}");
                    builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces();
                });

                #endregion

                #region Serializers

                builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
                #if UNIT_NEWTONSOFT_JSON
                builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
                #endif
                serializerTypes?.ForEach(type =>
                {
                    if (!typeof(ISerializer).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement {nameof(ISerializer)}");
                    builder.Register(type, Lifetime.Singleton).AsImplementedInterfaces();
                });

                #endregion

                builder.Register<DataManager>(Lifetime.Singleton).AsImplementedInterfaces();
            }

            #endregion

            #region UI

            if (rootUICanvas is { })
            {
                builder.RegisterInstance(rootUICanvas).AsSelf();
                builder.Register<UIManager>(Lifetime.Singleton).AsImplementedInterfaces();
            }

            #endregion

            #region Utilities

            builder.Register<ObjectPoolManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AudioManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityManager>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion
        }
    }
}
#endif