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

    public class AddressableAssetManager : IAssetManager
    {
        #region Constructor

        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets;
        private readonly ILogger                                  _logger;

        [Preserve]
        public AddressableAssetManager(ILogger logger = null)
        {
            this._loadedAssets = new();
            this._logger       = logger ?? ILogger.Default(this.GetType().Name);
        }

        #endregion

        #region Finalizer

        ~AddressableAssetManager() => this.Dispose();

        public void Dispose()
        {
            this._loadedAssets.Keys.SafeForEach(this.Unload);
        }

        #endregion

        #region Public

        public LogConfig LogConfig => this._logger.Config;

        public UniTask<T> Load<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            key ??= typeof(T).GetKey();
            return this._loadedAssets.GetOrAdd(key, () => Addressables.LoadAssetAsync<T>(key))
                       .Convert<T>()
                       .ToUniTask(progress: progress, cancellationToken: cancellationToken)
                       .ContinueWith(asset =>
                       {
                           this._logger.Debug($"Loaded asset {key}");
                           return asset;
                       })
                       .Catch(new Func<Exception, T>(exception =>
                       {
                           this._logger.Exception(exception);
                           throw exception;
                       }));
        }

        public UniTask<T> LoadComponent<T>(string key = null, IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : Component
        {
            return this.Load<GameObject>(key ?? typeof(T).GetKey(), progress, cancellationToken)
                       .ContinueWith(gameObject => gameObject.GetComponent<T>() ?? throw new InvalidOperationException($"Component {typeof(T).Name} not found in GameObject {gameObject.name}"))
                       .Catch(new Func<Exception, T>(exception =>
                       {
                           this._logger.Exception(exception);
                           throw exception;
                       }));
        }

        public void Unload(string key)
        {
            if (!this._loadedAssets.Remove(key, out var handle))
            {
                this._logger.Warning($"Trying to unload asset {key} that was not loaded");
                return;
            }
            Addressables.Release(handle);
            this._logger.Debug($"Unloaded asset {key}");
        }

        public void Unload<T>()
        {
            this.Unload(typeof(T).GetKey());
        }

        #endregion
    }
}