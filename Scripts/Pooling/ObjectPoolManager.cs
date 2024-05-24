#nullable enable
namespace UniT.Pooling
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        private readonly Dictionary<string, GameObject>     keyToPrefab    = new Dictionary<string, GameObject>();
        private readonly Dictionary<GameObject, ObjectPool> prefabToPool   = new Dictionary<GameObject, ObjectPool>();
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

        void IObjectPoolManager.Load(GameObject prefab, int count) => this.Load(prefab, count);

        void IObjectPoolManager.Load(string key, int count)
        {
            var prefab = this.keyToPrefab.GetOrAdd(key, () => this.assetsManager.Load<GameObject>(key));
            this.Load(prefab, count);
        }

        #if UNIT_UNITASK
        async UniTask IObjectPoolManager.LoadAsync(string key, int count, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var prefab = await this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken);
            this.Load(prefab, count);
        }
        #else
        IEnumerator IObjectPoolManager.LoadAsync(string key, int count, Action? callback, IProgress<float>? progress)
        {
            var prefab = default(GameObject);
            yield return this.assetsManager.LoadAsync<GameObject>(
                key,
                result => prefab = result,
                progress
            );
            this.Load(prefab, count);
            callback?.Invoke();
        }
        #endif

        GameObject IObjectPoolManager.Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform? parent) => this.Spawn(prefab, position, rotation, parent);

        GameObject IObjectPoolManager.Spawn(string key, Vector3 position, Quaternion rotation, Transform? parent)
        {
            var prefab = this.keyToPrefab.GetOrAdd(key, () => this.assetsManager.Load<GameObject>(key));
            return this.Spawn(prefab, position, rotation, parent);
        }

        void IObjectPoolManager.Recycle(GameObject instance)
        {
            if (!this.instanceToPool.TryRemove(instance, out var pool)) throw new InvalidOperationException($"Trying to recycle {instance.name} that is not spawned");
            pool.Recycle(instance);
            this.logger.Debug($"Recycled {instance.name}");
        }

        void IObjectPoolManager.RecycleAll(GameObject prefab) => this.RecycleAll(prefab);

        void IObjectPoolManager.RecycleAll(string key)
        {
            if (!this.TryGetPrefab(key, out var prefab)) return;
            this.RecycleAll(prefab);
        }

        void IObjectPoolManager.Cleanup(GameObject prefab, int retainCount) => this.Cleanup(prefab, retainCount);

        void IObjectPoolManager.Cleanup(string key, int retainCount)
        {
            if (!this.TryGetPrefab(key, out var prefab)) return;
            this.Cleanup(prefab, retainCount);
        }

        void IObjectPoolManager.Unload(GameObject prefab) => this.Unload(prefab);

        void IObjectPoolManager.Unload(string key)
        {
            if (!this.TryGetPrefab(key, out var prefab)) return;
            this.Unload(prefab);
            this.assetsManager.Unload(key);
        }

        #endregion

        #region Private

        private void Load(GameObject prefab, int count)
        {
            this.prefabToPool.GetOrAdd(prefab, () =>
            {
                var pool = ObjectPool.Construct(prefab, this.poolsContainer);
                this.logger.Debug($"Instantiated {pool.gameObject.name}");
                return pool;
            }).Load(count);
        }

        private GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform? parent)
        {
            if (!this.prefabToPool.ContainsKey(prefab))
            {
                this.Load(prefab, 1);
                this.logger.Warning($"Auto loading {prefab.name} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
            }
            var pool     = this.prefabToPool[prefab];
            var instance = pool.Spawn(position, rotation, parent);
            this.instanceToPool.Add(instance, pool);
            this.logger.Debug($"Spawned {prefab.name}");
            return instance;
        }

        private void RecycleAll(GameObject prefab)
        {
            if (!this.TryGetPool(prefab, out var pool)) return;
            pool.RecycleAll();
            this.instanceToPool.RemoveAll((_, otherPool) => otherPool == pool);
            this.logger.Debug($"Recycled all {prefab.name}");
        }

        private void Cleanup(GameObject prefab, int retainCount)
        {
            if (!this.TryGetPool(prefab, out var pool)) return;
            pool.Cleanup(retainCount);
            this.logger.Debug($"Cleaned up {pool.gameObject.name}");
        }

        private void Unload(GameObject prefab)
        {
            if (!this.TryGetPool(prefab, out var pool)) return;
            this.RecycleAll(prefab);
            Object.Destroy(pool.gameObject);
            this.logger.Debug($"Destroyed {pool.gameObject.name}");
        }

        private bool TryGetPool(GameObject prefab, [MaybeNullWhen(false)] out ObjectPool pool)
        {
            if (this.prefabToPool.TryGet(prefab, out pool)) return true;
            this.logger.Warning($"{prefab.name} pool not loaded");
            return false;
        }

        private bool TryGetPrefab(string key, [MaybeNullWhen(false)] out GameObject prefab)
        {
            if (this.keyToPrefab.TryGet(key, out prefab)) return true;
            this.logger.Warning($"{key} pool not loaded");
            return false;
        }

        #endregion
    }
}