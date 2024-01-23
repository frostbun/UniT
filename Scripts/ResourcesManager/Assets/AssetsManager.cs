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
            var isLoaded = this.cache.TryAdd(
                key,
                () => this.Load(key) ?? throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}")
            );
            this.logger.Debug(isLoaded ? $"Using cached {key}" : $"Loaded {key}");
            return this.cache[key] as T ?? throw new InvalidCastException($"Failed to cast {key} to {typeof(T).Name}");
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
        async UniTask<T> IAssetsManager.LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var isLoaded = await this.cache.TryAddAsync(
                key,
                async () => await this.LoadAsync(key, progress, cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}")
            );
            this.logger.Debug(isLoaded ? $"Using cached {key}" : $"Loaded {key}");
            return this.cache[key] as T ?? throw new InvalidCastException($"Failed to cast {key} to {typeof(T).Name}");
        }

        protected abstract UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken);
        #else
        IEnumerator IAssetsManager.LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress)
        {
            yield return this.cache.TryAddAsync(
                key,
                callback => this.LoadAsync(
                    key,
                    obj => callback(obj ?? throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}")),
                    progress
                ),
                isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Loaded {key}" : $"Using cached {key}");
                    callback(this.cache[key] as T ?? throw new InvalidCastException($"Failed to cast {key} to {typeof(T).Name}"));
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