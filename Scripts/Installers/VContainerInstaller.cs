#if UNIT_VCONTAINER
namespace UniT.Installers
{
    using System;
    using System.Collections.Generic;
    using UniT.Audio;
    using UniT.Data;
    using UniT.ECC;
    using UniT.Extensions;
    using UniT.Instantiator;
    using UniT.Logging;
    using UniT.Pooling;
    using UniT.ResourcesManager;
    using UniT.UI;
    using VContainer;

    public static class VContainerInstaller
    {
        public static void RegisterUniT(this IContainerBuilder builder, IEnumerable<Type> dataTypes = null, RootUICanvas rootUICanvas = null, LogLevel logLevel = LogLevel.Info)
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
                #region Storages

                builder.Register<AssetDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
                builder.Register<PlayerPrefsDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
                #if UNIT_FBINSTANT && UNITY_WEBGL && !UNITY_EDITOR
                container.Register<FbInstantDataStorage>(Lifetime.Singleton).AsImplementedInterfaces();
                #endif

                #endregion

                #region Serializers

                builder.Register<CsvSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
                #if UNIT_NEWTONSOFT_JSON
                builder.Register<JsonSerializer>(Lifetime.Singleton).AsImplementedInterfaces();
                #endif

                #endregion

                dataTypes.ForEach(type =>
                {
                    if (!typeof(IData).IsAssignableFrom(type)) throw new ArgumentException($"{type} does not implement IData");
                    builder.Register(type, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
                });
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

            builder.Register<AudioManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ObjectPoolManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<EntityManager>(Lifetime.Singleton).AsImplementedInterfaces();

            #endregion
        }
    }
}
#endif