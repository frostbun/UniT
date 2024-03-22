namespace UniT.ResourcesManager
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.Logging;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class AssetsManager : IAssetsManager, IHasLogger, IDisposable
    {
        #region Constructor

        private readonly ILogger logger;

        private readonly Dictionary<string, Object> cache = new Dictionary<string, Object>();

        protected AssetsManager(ILogger.IFactory loggerFactory)
        {
            this.logger = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        #region Sync

        T IAssetsManager.Load<T>(string key)
        {
            var isLoaded = this.cache.TryAdd(
                key,
                () => this.Load<T>(key) ?? throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}")
            );
            this.logger.Debug(isLoaded ? $"Using cached {key}" : $"Loaded {key}");
            return (T)this.cache[key];
        }

        void IAssetsManager.Unload(string key)
        {
            if (this.cache.TryRemove(key, out var obj))
            {
                this.Unload(obj);
                this.logger.Debug($"Unloaded {key}");
            }
            else
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
            }
        }

        protected abstract Object Load<T>(string key) where T : Object;

        protected abstract void Unload(Object obj);

        #endregion

        #region Async

        #if UNIT_UNITASK
        async UniTask<T> IAssetsManager.LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var isLoaded = await this.cache.TryAddAsync(
                key,
                async () => await this.LoadAsync<T>(key, progress, cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}")
            );
            this.logger.Debug(isLoaded ? $"Using cached {key}" : $"Loaded {key}");
            return (T)this.cache[key];
        }

        protected abstract UniTask<Object> LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken) where T : Object;
        #else
        IEnumerator IAssetsManager.LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress)
        {
            yield return this.cache.TryAddAsync(
                key,
                callback => this.LoadAsync<T>(
                    key,
                    obj => callback(obj ?? throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}")),
                    progress
                ),
                isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Loaded {key}" : $"Using cached {key}");
                    callback((T)this.cache[key]);
                }
            );
        }

        protected abstract IEnumerator LoadAsync<T>(string key, Action<Object> callback, IProgress<float> progress) where T : Object;
        #endif

        #endregion

        #region Finalizer

        private void Dispose()
        {
            this.cache.Clear((_, obj) => this.Unload(obj));
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
            this.logger.Debug("Disposed");
        }

        ~AssetsManager()
        {
            this.Dispose();
            this.logger.Debug("Finalized");
        }

        #endregion
    }
}