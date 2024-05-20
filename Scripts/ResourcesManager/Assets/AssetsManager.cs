namespace UniT.ResourcesManager
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.Logging;
    using UnityEngine;
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

        protected AssetsManager(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Sync

        T IAssetsManager.Load<T>(string key) => this.LoadOrThrow<T>(key);

        bool IAssetsManager.TryLoad<T>(string key, out T asset)
        {
            try
            {
                asset = this.LoadOrThrow<T>(key);
                return true;
            }
            catch
            {
                asset = null;
                return false;
            }
        }

        T IAssetsManager.LoadComponent<T>(string key) => this.LoadComponentOrThrow<T>(key);

        bool IAssetsManager.TryLoadComponent<T>(string key, out T component)
        {
            try
            {
                component = this.LoadComponentOrThrow<T>(key);
                return true;
            }
            catch
            {
                component = default;
                return false;
            }
        }

        private T LoadOrThrow<T>(string key) where T : Object
        {
            try
            {
                return (T)this.cache.GetOrAdd(key, () =>
                {
                    var asset = this.Load<T>(key);
                    if (asset is null) throw new NullReferenceException($"{key} is null");
                    this.logger.Debug($"Loaded {key}");
                    return asset;
                });
            }
            catch (Exception inner)
            {
                throw new ArgumentOutOfRangeException($"Failed to load {key}", inner);
            }
        }

        private T LoadComponentOrThrow<T>(string key)
        {
            return this.LoadOrThrow<GameObject>(key).GetComponentOrThrow<T>();
        }

        protected abstract Object Load<T>(string key) where T : Object;

        protected abstract void Unload(Object asset);

        #endregion

        #region Async

        #if UNIT_UNITASK
        UniTask<T> IAssetsManager.LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadOrThrowAsync<T>(key, progress, cancellationToken);

        UniTask<(bool IsSucceeded, T Asset)> IAssetsManager.TryLoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.LoadOrThrowAsync<T>(
                    key,
                    progress,
                    cancellationToken
                )
                .ContinueWith(asset => (true, asset))
                .Catch(() => (false, null));
        }

        UniTask<T> IAssetsManager.LoadComponentAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadComponentOrThrowAsync<T>(key, progress, cancellationToken);

        UniTask<(bool IsSucceeded, T Component)> IAssetsManager.TryLoadComponentAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.LoadComponentOrThrowAsync<T>(
                    key,
                    progress,
                    cancellationToken
                )
                .ContinueWith(component => (true, component))
                .Catch(() => (false, default));
        }

        private UniTask<T> LoadOrThrowAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken) where T : Object
        {
            return this.cache.GetOrAddAsync(key, () =>
                this.LoadAsync<T>(key, progress, cancellationToken)
                    .ContinueWith(asset =>
                    {
                        if (asset is null) throw new ArgumentOutOfRangeException(nameof(key), key, $"Failed to load {key}");
                        this.logger.Debug($"Loaded {key}");
                        return asset;
                    })
            ).ContinueWith(asset => (T)asset);
        }

        private UniTask<T> LoadComponentOrThrowAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.LoadOrThrowAsync<GameObject>(key, progress, cancellationToken)
                .ContinueWith(gameObject => gameObject.GetComponentOrThrow<T>());
        }

        protected abstract UniTask<Object> LoadAsync<T>(string key, IProgress<float> progress, CancellationToken cancellationToken) where T : Object;
        #else
        IEnumerator IAssetsManager.LoadAsync<T>(string key, Action<T> callback, IProgress<float> progress) => this.LoadOrThrowAsync(key, callback, progress);

        IEnumerator IAssetsManager.TryLoadAsync<T>(string key, Action<(bool IsSucceeded, T Asset)> callback, IProgress<float> progress)
        {
            return this.LoadOrThrowAsync<T>(
                key,
                asset => callback((true, asset)),
                progress
            ).Catch(() => callback((false, null)));
        }

        IEnumerator IAssetsManager.LoadComponentAsync<T>(string key, Action<T> callback, IProgress<float> progress) => this.LoadComponentOrThrowAsync(key, callback, progress);

        IEnumerator IAssetsManager.TryLoadComponentAsync<T>(string key, Action<(bool IsSucceeded, T Component)> callback, IProgress<float> progress)
        {
            return this.LoadComponentOrThrowAsync<T>(
                key,
                component => callback((true, component)),
                progress
            ).Catch(() => callback((false, default)));
        }

        private IEnumerator LoadOrThrowAsync<T>(string key, Action<T> callback, IProgress<float> progress) where T : Object
        {
            return this.cache.GetOrAddAsync(
                key,
                callback => this.LoadAsync<T>(
                    key,
                    asset =>
                    {
                        if (asset is null) throw new NullReferenceException($"{key} is null");
                        this.logger.Debug($"Loaded {key}");
                        callback(asset);
                    },
                    progress
                ),
                asset => callback((T)asset)
            ).Catch(inner => throw new ArgumentOutOfRangeException($"Failed to load {key}", inner));
        }

        private IEnumerator LoadComponentOrThrowAsync<T>(string key, Action<T> callback, IProgress<float> progress)
        {
            return this.LoadOrThrowAsync<GameObject>(
                key,
                gameObject => callback(gameObject.GetComponentOrThrow<T>()),
                progress
            );
        }

        protected abstract IEnumerator LoadAsync<T>(string key, Action<Object> callback, IProgress<float> progress) where T : Object;
        #endif

        #endregion

        #region Finalizer

        void IAssetsManager.Unload(string key)
        {
            if (!this.cache.TryRemove(key, out var asset))
            {
                this.logger.Warning($"Trying to unload {key} that was not loaded");
                return;
            }
            this.Unload(asset);
            this.logger.Debug($"Unloaded {key}");
        }

        private void Dispose()
        {
            this.cache.Clear((_, asset) => this.Unload(asset));
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