namespace UniT.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;

    public sealed class AddressableAssetsManager : IAssetsManager
    {
        #region Constructor

        private readonly Dictionary<string, AsyncOperationHandle> loadedAssets;
        private readonly ILogger                                  logger;

        [Preserve]
        public AddressableAssetsManager(ILogger logger = null)
        {
            this.loadedAssets = new();
            this.logger       = logger ?? ILogger.Default(this.GetType().Name);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Finalizer

        ~AddressableAssetsManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        public void Dispose()
        {
            this.loadedAssets.Keys.SafeForEach(this.Unload);
            this.logger.Debug("Disposed");
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this.logger.Config;

        public UniTask<T> Load<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            key ??= typeof(T).GetKey();
            return this.loadedAssets
                .GetOrAdd(key, () => Addressables.LoadAssetAsync<T>(key))
                .Convert<T>()
                .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                .ContinueWith(asset =>
                {
                    this.logger.Debug($"Loaded asset {key}");
                    return asset;
                });
        }

        public UniTask<T> LoadComponent<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Component
        {
            key ??= typeof(T).GetKey();
            return this.Load<GameObject>(key, progress, cancellationToken)
                .ContinueWith(
                    gameObject => gameObject.GetComponent<T>()
                        ?? throw new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {key}")
                );
        }

        public void Unload(string key)
        {
            if (!this.loadedAssets.Remove(key, out var handle))
            {
                this.logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            Addressables.Release(handle);
            this.logger.Debug($"Unloaded asset {key}");
        }

        public void Unload<T>()
        {
            this.Unload(typeof(T).GetKey());
        }

        #endregion
    }
}