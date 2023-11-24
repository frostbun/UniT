namespace UniT.ResourcesManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.Logging;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public abstract class AssetsManager : IAssetsManager
    {
        #region Constructor

        private readonly Dictionary<string, Object> cache;
        private readonly ILogger                    logger;

        protected AssetsManager(ILogger logger = null)
        {
            this.cache  = new();
            this.logger = logger ?? ILogger.Default(this.GetType().Name);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        LogConfig IAssetsManager.LogConfig => this.logger.Config;

        T IAssetsManager.Load<T>(string key)
        {
            if (this.cache.ContainsKey(key))
            {
                this.logger.Debug($"Using cached {key}");
            }
            else
            {
                this.cache.Add(key, this.Load(key));
                this.logger.Debug($"Loaded {key}");
            }
            return (T)this.cache[key];
        }

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

        void IAssetsManager.Unload(string key)
        {
            if (this.cache.Remove(key, out var @object))
            {
                this.Unload(@object);
                this.logger.Debug($"Unloaded {key}");
            }
            else
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
            }
        }

        #endregion

        #region Private

        protected abstract Object Load(string key);

        protected abstract UniTask<Object> LoadAsync(string key, IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract void Unload(Object @object);

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