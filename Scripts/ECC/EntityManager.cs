namespace UniT.ECC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.ECC.Component;
    using UniT.ECC.Controller;
    using UniT.ECC.Entity;
    using UniT.Extensions;
    using UniT.Instantiator;
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

    public sealed class EntityManager : IEntityManager, IDisposable
    {
        #region Constructor

        private readonly IInstantiator  instantiator;
        private readonly IAssetsManager assetsManager;
        private readonly ILogger        logger;

        private readonly Transform                             poolsContainer   = new GameObject(nameof(EntityManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<IEntity, EntityPool>       prefabToPool     = new Dictionary<IEntity, EntityPool>();
        private readonly Dictionary<string, EntityPool>        keyToPool        = new Dictionary<string, EntityPool>();
        private readonly Dictionary<IEntity, EntityPool>       entityToPool     = new Dictionary<IEntity, EntityPool>();
        private readonly Dictionary<Type, HashSet<IComponent>> typeToComponents = new Dictionary<Type, HashSet<IComponent>>();

        [Preserve]
        public EntityManager(IInstantiator instantiator, IAssetsManager assetsManager, ILoggerManager loggerManager)
        {
            this.instantiator  = instantiator;
            this.assetsManager = assetsManager;
            this.logger        = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        #region Pooling

        void IEntityManager.Load(IEntity prefab, int count)
        {
            this.ThrowIfDisposed();
            this.prefabToPool.GetOrAdd(prefab, () => new EntityPool(prefab, this))
                .Load(count);
        }

        void IEntityManager.Load(string key, int count)
        {
            this.ThrowIfDisposed();
            this.keyToPool.GetOrAdd(key, () => new EntityPool(this.assetsManager.Load<GameObject>(key).GetComponentOrThrow<IEntity>(), this))
                .Load(count);
        }

        #if UNIT_UNITASK
        UniTask IEntityManager.LoadAsync(string key, int count, IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();
            return this.keyToPool.GetOrAddAsync(key, () =>
                this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken)
                    .ContinueWith(prefab => new EntityPool(prefab.GetComponentOrThrow<IEntity>(), this))
            ).ContinueWith(pool => pool.Load(count));
        }
        #else
        IEnumerator IEntityManager.LoadAsync(string key, int count, Action callback, IProgress<float> progress)
        {
            this.ThrowIfDisposed();
            return this.keyToPool.GetOrAddAsync(
                key,
                callback => this.assetsManager.LoadAsync<GameObject>(key, prefab => callback(new EntityPool(prefab.GetComponentOrThrow<IEntity>(), this))),
                pool =>
                {
                    pool.Load(count);
                    callback?.Invoke();
                }
            );
        }
        #endif

        TEntity IEntityManager.Spawn<TEntity>(TEntity prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPoolOrLoad(prefab).Spawn<TEntity>(position, rotation, parent);
        }

        TEntity IEntityManager.Spawn<TEntity, TParams>(TEntity prefab, TParams @params, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPoolOrLoad(prefab).Spawn<TEntity, TParams>(@params, position, rotation, parent);
        }

        TEntity IEntityManager.Spawn<TEntity>(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPoolOrLoad(key).Spawn<TEntity>(position, rotation, parent);
        }

        TEntity IEntityManager.Spawn<TEntity, TParams>(string key, TParams @params, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPoolOrLoad(key).Spawn<TEntity, TParams>(@params, position, rotation, parent);
        }

        void IEntityManager.Recycle(IEntity entity)
        {
            this.ThrowIfDisposed();
            if (!this.entityToPool.TryGet(entity, out var pool)) throw new InvalidOperationException($"Trying to recycle {entity.Name} that is not spawned");
            pool.Recycle(entity);
        }

        void IEntityManager.RecycleAll(IEntity prefab)
        {
            this.GetPoolOrWarning(prefab)?.RecycleAll();
        }

        void IEntityManager.RecycleAll(string key)
        {
            this.GetPoolOrWarning(key)?.RecycleAll();
        }

        void IEntityManager.Cleanup(IEntity prefab, int retainCount)
        {
            this.GetPoolOrWarning(prefab)?.Cleanup(retainCount);
        }

        void IEntityManager.Cleanup(string key, int retainCount)
        {
            this.GetPoolOrWarning(key)?.Cleanup(retainCount);
        }

        void IEntityManager.Unload(IEntity prefab)
        {
            this.ThrowIfDisposed();
            if (!this.prefabToPool.TryRemove(prefab, out var pool))
            {
                this.logger.Warning($"Trying to unload {prefab.Name} pool that is not loaded");
                return;
            }
            pool.Dispose();
        }

        void IEntityManager.Unload(string key)
        {
            this.ThrowIfDisposed();
            if (!this.keyToPool.TryRemove(key, out var pool))
            {
                this.logger.Warning($"Trying to unload {key} pool that is not loaded");
                return;
            }
            pool.Dispose();
            this.assetsManager.Unload(key);
        }

        private EntityPool GetPoolOrLoad(IEntity prefab)
        {
            this.ThrowIfDisposed();
            return this.prefabToPool.GetOrAdd(prefab, () =>
            {
                this.logger.Warning($"Auto loading {prefab.Name} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
                return new EntityPool(prefab, this);
            });
        }

        private EntityPool GetPoolOrLoad(string key)
        {
            this.ThrowIfDisposed();
            return this.keyToPool.GetOrAdd(key, () =>
            {
                this.logger.Warning($"Auto loading {key} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
                return new EntityPool(this.assetsManager.Load<GameObject>(key).GetComponentOrThrow<IEntity>(), this);
            });
        }

        private EntityPool GetPoolOrWarning(IEntity prefab)
        {
            this.ThrowIfDisposed();
            if (!this.prefabToPool.TryGet(prefab, out var pool))
            {
                this.logger.Warning($"{prefab.Name} pool not loaded");
            }
            return pool;
        }

        private EntityPool GetPoolOrWarning(string key)
        {
            if (!this.keyToPool.TryGet(key, out var pool))
            {
                this.logger.Warning($"{key} pool not loaded");
            }
            return pool;
        }

        private class EntityPool
        {
            private readonly IEntity       prefab;
            private readonly EntityManager manager;
            private readonly Transform     entitiesContainer;

            private readonly Queue<IEntity>                    pooledEntities     = new Queue<IEntity>();
            private readonly HashSet<IEntity>                  spawnedEntities    = new HashSet<IEntity>();
            private readonly Dictionary<IEntity, IComponent[]> entityToComponents = new Dictionary<IEntity, IComponent[]>();
            private readonly Dictionary<IComponent, Type[]>    componentToTypes   = new Dictionary<IComponent, Type[]>();

            public EntityPool(IEntity prefab, EntityManager manager)
            {
                this.prefab  = prefab;
                this.manager = manager;
                this.entitiesContainer = new GameObject
                {
                    name      = $"{prefab.Name} pool",
                    transform = { parent = manager.poolsContainer },
                }.transform;
            }

            public void Load(int count)
            {
                while (this.pooledEntities.Count < count) this.pooledEntities.Enqueue(this.Instantiate());
            }

            public TEntity Spawn<TEntity>(Vector3 position, Quaternion rotation, Transform parent) where TEntity : IEntityWithoutParams
            {
                var entity = this.pooledEntities.DequeueOrDefault(this.Instantiate);
                entity.Transform.SetPositionAndRotation(position, rotation);
                entity.Transform.SetParent(parent);
                entity.GameObject.SetActive(true);
                this.spawnedEntities.Add(entity);
                this.manager.entityToPool.Add(entity, this);
                this.entityToComponents[entity].ForEach(component =>
                {
                    this.componentToTypes[component].ForEach(type => this.manager.Register(type, component));
                    component.OnSpawn();
                });
                return (TEntity)entity;
            }

            public TEntity Spawn<TEntity, TParams>(TParams @params, Vector3 position, Quaternion rotation, Transform parent) where TEntity : IEntityWithParams<TParams>
            {
                var entity = this.pooledEntities.DequeueOrDefault(this.Instantiate);
                entity.Transform.SetPositionAndRotation(position, rotation);
                entity.Transform.SetParent(parent);
                entity.GameObject.SetActive(true);
                this.spawnedEntities.Add(entity);
                this.manager.entityToPool.Add(entity, this);
                ((IEntityWithParams<TParams>)entity).Params = @params;
                this.entityToComponents[entity].ForEach(component =>
                {
                    this.componentToTypes[component].ForEach(type => this.manager.Register(type, component));
                    component.OnSpawn();
                });
                return (TEntity)entity;
            }

            public void Recycle(IEntity entity)
            {
                this.entityToComponents[entity].ForEach(component =>
                {
                    component.OnRecycle();
                    this.componentToTypes[component].ForEach(type => this.manager.Unregister(type, component));
                });
                this.manager.entityToPool.Remove(entity);
                this.spawnedEntities.Remove(entity);
                if (entity.IsDestroyed) return;
                entity.GameObject.SetActive(false);
                entity.Transform.SetParent(this.entitiesContainer);
                this.pooledEntities.Enqueue(entity);
            }

            public void RecycleAll()
            {
                this.spawnedEntities.SafeForEach(this.Recycle);
            }

            public void Cleanup(int retainCount)
            {
                while (this.pooledEntities.Count > retainCount)
                {
                    var entity = this.pooledEntities.Dequeue();
                    this.entityToComponents.Remove(entity, out var components);
                    components.ForEach(component => this.componentToTypes.Remove(component));
                    Object.Destroy(entity.GameObject);
                }
            }

            public void Dispose()
            {
                this.RecycleAll();
                this.pooledEntities.Clear();
                Object.Destroy(this.entitiesContainer.gameObject);
            }

            private IEntity Instantiate()
            {
                var entity = Object.Instantiate(this.prefab.GameObject, this.entitiesContainer).GetComponent<IEntity>();
                entity.GameObject.SetActive(false);
                this.entityToComponents.Add(entity, entity.GetComponentsInChildren<IComponent>());
                this.entityToComponents[entity].ForEach(component =>
                {
                    component.Manager = this.manager;
                    if (component is IHasController owner)
                    {
                        var controller = (IController)this.manager.instantiator.Instantiate(owner.ControllerType);
                        controller.Owner = owner;
                        owner.Controller = controller;
                    }
                    component.OnInstantiate();
                    this.componentToTypes.Add(
                        component,
                        component.GetType()
                            .GetInterfaces()
                            .Where(type => !typeof(IComponent).IsAssignableFrom(type))
                            .Where(type => !typeof(IHasController).IsAssignableFrom(type))
                            .Prepend(component.GetType())
                            .ToArray()
                    );
                });
                return entity;
            }
        }

        #endregion

        #region Query

        IEnumerable<T> IEntityManager.Query<T>()
        {
            this.ThrowIfDisposed();
            return this.typeToComponents.GetOrAdd(typeof(T)).Cast<T>();
        }

        private void Register(Type type, IComponent component)
        {
            this.typeToComponents.GetOrAdd(type).Add(component);
        }

        private void Unregister(Type type, IComponent component)
        {
            this.typeToComponents.GetOrAdd(type).Remove(component);
        }

        #endregion

        #region Finalizer

        private bool isDisposed = false;

        private void ThrowIfDisposed()
        {
            if (this.isDisposed) throw new ObjectDisposedException(nameof(EntityManager));
        }

        private void Dispose()
        {
            if (!this.poolsContainer)
            {
                this.isDisposed = true;
                return;
            }
            this.prefabToPool.Clear((_, pool) =>
            {
                pool.Dispose();
            });
            this.keyToPool.Clear((key, pool) =>
            {
                pool.Dispose();
                this.assetsManager.Unload(key);
            });
            this.typeToComponents.Clear();
            Object.Destroy(this.poolsContainer.gameObject);
            this.isDisposed = true;
            this.logger.Debug("Disposed");
        }

        void IDisposable.Dispose()
        {
            this.ThrowIfDisposed();
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        ~EntityManager()
        {
            this.Dispose();
        }

        #endregion
    }
}