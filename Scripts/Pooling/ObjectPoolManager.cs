namespace UniT.Pooling
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.ResourcesManager;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class ObjectPoolManager : IObjectPoolManager
    {
        #region Constructor

        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly Transform                          poolsContainer = new GameObject(nameof(ObjectPoolManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool   = new Dictionary<GameObject, ObjectPool>();
        private readonly Dictionary<string, ObjectPool>     keyToPool      = new Dictionary<string, ObjectPool>();
        private readonly Dictionary<GameObject, ObjectPool> instanceToPool = new Dictionary<GameObject, ObjectPool>();

        [Preserve]
        public ObjectPoolManager(IAssetsManager assetsManager, ILoggerManager loggerManager)
        {
            this.assetsManager = assetsManager;
            this.logger        = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Public

        void IObjectPoolManager.Load(GameObject prefab, int count)
        {
            this.prefabToPool.GetOrAdd(prefab, () => this.Load(prefab))
                .Load(count);
        }

        void IObjectPoolManager.Load(string key, int count)
        {
            this.keyToPool.GetOrAdd(key, () => this.Load(this.assetsManager.Load<GameObject>(key)))
                .Load(count);
        }

        #if UNIT_UNITASK
        UniTask IObjectPoolManager.LoadAsync(string key, int count, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return this.keyToPool.GetOrAddAsync(key, () =>
                this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken)
                    .ContinueWith(this.Load)
            ).ContinueWith(pool => pool.Load(count));
        }
        #else
        IEnumerator IObjectPoolManager.LoadAsync(string key, int count, Action callback, IProgress<float> progress)
        {
            return this.keyToPool.GetOrAddAsync(
                key,
                callback => this.assetsManager.LoadAsync<GameObject>(
                    key,
                    prefab => callback(this.Load(prefab)),
                    progress
                ),
                pool =>
                {
                    pool.Load(count);
                    callback?.Invoke();
                }
            );
        }
        #endif

        GameObject IObjectPoolManager.Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool = this.prefabToPool.GetOrAdd(prefab, () =>
            {
                this.logger.Warning($"Auto loading {prefab.name} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
                return this.Load(prefab);
            });
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.logger.Debug($"Spawned {prefab.name}");
            return instance;
        }

        GameObject IObjectPoolManager.Spawn(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool = this.keyToPool.GetOrAdd(key, () =>
            {
                this.logger.Warning($"Auto loading {key} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
                return this.Load(this.assetsManager.Load<GameObject>(key));
            });
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.logger.Debug($"Spawned {key}");
            return instance;
        }

        void IObjectPoolManager.Recycle(GameObject instance)
        {
            if (!this.instanceToPool.TryRemove(instance, out var pool))
            {
                this.logger.Warning($"Trying to recycle {instance.name} that was not spawned from {this.GetType().Name}");
                return;
            }
            this.Recycle(instance, pool);
        }

        void IObjectPoolManager.RecycleAll(GameObject prefab)
        {
            if (this.GetPoolOrWarning(prefab) is not { } pool) return;
            this.RecycleAll(pool);
        }

        void IObjectPoolManager.RecycleAll(string key)
        {
            if (this.GetPoolOrWarning(key) is not { } pool) return;
            this.RecycleAll(pool);
        }

        void IObjectPoolManager.Cleanup(GameObject prefab, int retainCount)
        {
            this.GetPoolOrWarning(prefab)?.Cleanup(retainCount);
        }

        void IObjectPoolManager.Cleanup(string key, int retainCount)
        {
            this.GetPoolOrWarning(key)?.Cleanup(retainCount);
        }

        void IObjectPoolManager.Unload(GameObject prefab)
        {
            if (this.GetPoolOrWarning(prefab) is not { } pool) return;
            this.Unload(pool);
        }

        void IObjectPoolManager.Unload(string key)
        {
            if (this.GetPoolOrWarning(key) is not { } pool) return;
            this.Unload(pool);
            this.assetsManager.Unload(key);
        }

        #endregion

        #region Private

        private ObjectPool GetPoolOrWarning(GameObject prefab)
        {
            if (!this.prefabToPool.TryGet(prefab, out var pool))
            {
                this.logger.Warning($"{prefab.name} pool not loaded");
            }
            return pool;
        }

        private ObjectPool GetPoolOrWarning(string key)
        {
            if (!this.keyToPool.TryGet(key, out var pool))
            {
                this.logger.Warning($"{key} pool not loaded");
            }
            return pool;
        }

        private ObjectPool Load(GameObject prefab)
        {
            var pool = ObjectPool.Instantiate(prefab);
            pool.transform.SetParent(this.poolsContainer);
            this.logger.Debug($"Instantiated {pool.gameObject.name}");
            return pool;
        }

        private void Recycle(GameObject instance, ObjectPool pool)
        {
            pool.Recycle(instance);
            if (!instance)
            {
                this.logger.Warning($"Trying to recycle {instance.name} that is destroyed");
                return;
            }
            this.logger.Debug($"Recycled {instance.name}");
        }

        private void RecycleAll(ObjectPool pool)
        {
            this.instanceToPool.RemoveAll((instance, otherPool) =>
            {
                if (otherPool != pool) return false;
                this.Recycle(instance, pool);
                return true;
            });
            this.logger.Debug($"Recycled all {pool.gameObject.name}");
        }

        private void Unload(ObjectPool pool)
        {
            this.RecycleAll(pool);
            Object.Destroy(pool.gameObject);
            this.logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        #endregion
    }
}