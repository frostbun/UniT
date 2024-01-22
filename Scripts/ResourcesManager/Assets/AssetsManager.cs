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

    public abstract class AssetsManager : IAssetsManager
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
            var isLoaded = this.cache.TryAdd(key, () => this.Load(key));
            this.logger.Debug(isLoaded ? $"Using cached {key}" : $"Loaded {key}");
            return (T)this.cache[key];
        }

        void IAssetsManager.Unload(string key)
        {
            if (this.cache.TryRemove(key, out var @object))
            {
                this.Unload(@object);
                this.logger.Debug($"Unloaded {key}");
            }
            else
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
            }
        }

        protected abstract Object Load(string key);

        protected abstract void Unload(Object @object);

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask<T> IAssetsManager.LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.cache
                .TryAddAsync(key, () => this.LoadAsync(key, progress, cancellationToken))
                .ContinueWith(isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Loaded {key}" : $"Using cached {key}");
                    return (T)this.cache[key];
                });
        }

        protected abstract UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken);
        #else
        IEnumerator IAssetsManager.LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress)
        {
            yield return this.cache.TryAddAsync(
                key,
                callback => this.LoadAsync(key, callback, progress),
                isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Loaded {key}" : $"Using cached {key}");
                    callback((T)this.cache[key]);
                });
        }

        protected abstract IEnumerator LoadAsync(string key, Action<Object> callback, IProgress<float> progress);
        #endif

        #endregion

        #region Finalizer

        private void Dispose()
        {
            this.cache.Clear((_, @object) => this.Unload(@object));
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