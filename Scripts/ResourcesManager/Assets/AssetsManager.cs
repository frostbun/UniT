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

        protected AssetsManager(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        #region Sync

        T IAssetsManager.Load<T>(string key)
        {
            return (T)this.cache.GetOrAdd(key, () =>
            {
                var obj = this.Load<T>(key);
                if (obj is null) throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}");
                this.logger.Debug($"Loaded {key}");
                return obj;
            });
        }

        void IAssetsManager.Unload(string key)
        {
            if (!this.cache.TryRemove(key, out var obj))
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
                return;
            }
            this.Unload(obj);
            this.logger.Debug($"Unloaded {key}");
        }

        protected abstract Object Load<T>(string key) where T : Object;

        protected abstract void Unload(Object obj);

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask<T> IAssetsManager.LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.cache.GetOrAddAsync(key, () =>
                this.LoadAsync<T>(key, progress, cancellationToken)
                    .ContinueWith(obj =>
                    {
                        if (obj is null) throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}");
                        this.logger.Debug($"Loaded {key}");
                        return obj;
                    })
            ).ContinueWith(obj => (T)obj);
        }

        protected abstract UniTask<Object> LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken) where T : Object;
        #else
        IEnumerator IAssetsManager.LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress)
        {
            return this.cache.GetOrAddAsync(
                key,
                callback => this.LoadAsync<T>(
                    key,
                    obj =>
                    {
                        if (obj is null) throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}");
                        this.logger.Debug($"Loaded {key}");
                        callback(obj);
                    },
                    progress
                ),
                obj => callback((T)obj)
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