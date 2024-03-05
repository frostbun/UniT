namespace UniT.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Entities.Controller;
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

    public sealed class EntityManager : IEntityManager
    {
        #region Constructor

        private readonly IController.IFactory controllerFactory;
        private readonly IAssetsManager       assetsManager;
        private readonly ILogger              logger;

        private readonly Transform                             poolsContainer   = new GameObject(nameof(EntityManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<IEntity, EntityPool>       prefabToPool     = new Dictionary<IEntity, EntityPool>();
        private readonly Dictionary<string, EntityPool>        keyToPool        = new Dictionary<string, EntityPool>();
        private readonly Dictionary<IEntity, EntityPool>       entityToPool     = new Dictionary<IEntity, EntityPool>();
        private readonly Dictionary<Type, HashSet<IComponent>> typeToComponents = new Dictionary<Type, HashSet<IComponent>>();

        [Preserve]
        public EntityManager(IController.IFactory controllerFactory, IAssetsManager assetsManager, ILogger.IFactory loggerFactory)
        {
            this.controllerFactory = controllerFactory;
            this.assetsManager     = assetsManager;
            this.logger            = loggerFactory.Create(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        LogConfig IHasLogger.LogConfig => this.logger.Config;

        #region Pooling

        void IEntityManager.Load(IEntity prefab, int count)
        {
            this.ThrowIfDisposed();
            var isLoaded = this.prefabToPool.TryAdd(prefab, () => new EntityPool(prefab, this));
            this.logger.Debug(isLoaded ? $"Loaded {prefab.gameObject.name} pool" : $"Using cached {prefab.gameObject.name} pool");
            this.prefabToPool[prefab].Load(count);
        }

        void IEntityManager.Load(string key, int count)
        {
            this.ThrowIfDisposed();
            var isLoaded = this.keyToPool.TryAdd(key, () =>
            {
                var prefab = this.assetsManager.Load<GameObject>(key);
                return new EntityPool(prefab.GetComponentOrThrow<IEntity>(), this);
            });
            this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
            this.keyToPool[key].Load(count);
        }

        #if UNIT_UNITASK
        async UniTask IEntityManager.LoadAsync(string key, int count, IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();
            var isLoaded = await this.keyToPool.TryAddAsync(key, async () =>
            {
                var prefab = await this.assetsManager.LoadAsync<GameObject>(key, progress, cancellationToken);
                return new EntityPool(prefab.GetComponentOrThrow<IEntity>(), this);
            });
            this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
            this.keyToPool[key].Load(count);
        }
        #else
        IEnumerator IEntityManager.LoadAsync(string key, int count, Action callback, IProgress<float> progress)
        {
            this.ThrowIfDisposed();
            yield return this.keyToPool.TryAddAsync(
                key,
                callback => this.assetsManager.LoadAsync<GameObject>(key, prefab => callback(new EntityPool(prefab.GetComponentOrThrow<IEntity>(), this))),
                isLoaded =>
                {
                    this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
                    this.keyToPool[key].Load(count);
                    callback?.Invoke();
                }
            );
        }
        #endif

        TEntity IEntityManager.Spawn<TEntity>(TEntity prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPool(prefab).Spawn<TEntity>(position, rotation, parent);
        }

        TEntity IEntityManager.Spawn<TEntity, TModel>(TEntity prefab, TModel model, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPool(prefab).Spawn<TEntity, TModel>(model, position, rotation, parent);
        }

        TEntity IEntityManager.Spawn<TEntity>(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPool(key).Spawn<TEntity>(position, rotation, parent);
        }

        TEntity IEntityManager.Spawn<TEntity, TModel>(string key, TModel model, Vector3 position, Quaternion rotation, Transform parent)
        {
            return this.GetPool(key).Spawn<TEntity, TModel>(model, position, rotation, parent);
        }

        void IEntityManager.Recycle(IEntity entity)
        {
            this.ThrowIfDisposed();
            if (!this.entityToPool.TryGet(entity, out var pool)) throw new InvalidOperationException($"Trying to recycle {entity.gameObject.name} that is not spawned");
            pool.Recycle(entity);
        }

        void IEntityManager.RecycleAll(IEntity prefab)
        {
            this.ThrowIfDisposed();
            if (!this.prefabToPool.TryGet(prefab, out var pool))
            {
                this.logger.Warning($"Trying to recycle all {prefab.gameObject.name} that is not loaded");
                return;
            }
            pool.RecycleAll();
        }

        void IEntityManager.RecycleAll(string key)
        {
            this.ThrowIfDisposed();
            if (!this.keyToPool.TryGet(key, out var pool))
            {
                this.logger.Warning($"Trying to recycle all {key} that is not loaded");
                return;
            }
            pool.RecycleAll();
        }

        void IEntityManager.Unload(IEntity prefab)
        {
            this.ThrowIfDisposed();
            if (!this.prefabToPool.TryRemove(prefab, out var pool))
            {
                this.logger.Warning($"Trying to unload {prefab.gameObject.name} pool that is not loaded");
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

        private EntityPool GetPool(IEntity prefab)
        {
            this.ThrowIfDisposed();
            var isLoaded = this.prefabToPool.TryAdd(prefab, () => new EntityPool(prefab, this));
            if (isLoaded) this.logger.Warning($"Auto loaded {prefab.gameObject.name} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
            return this.prefabToPool[prefab];
        }

        private EntityPool GetPool(string key)
        {
            this.ThrowIfDisposed();
            var isLoaded = this.keyToPool.TryAdd(key, () =>
            {
                var prefab = this.assetsManager.Load<GameObject>(key);
                return new EntityPool(prefab.GetComponentOrThrow<IEntity>(), this);
            });
            if (isLoaded) this.logger.Warning($"Auto loaded {key} pool. Consider preload it with `Load` or `LoadAsync` for better performance.");
            return this.keyToPool[key];
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
                    name      = $"{prefab.gameObject.name} pool",
                    transform = { parent = manager.poolsContainer },
                }.transform;
            }

            public void Load(int count)
            {
                while (this.pooledEntities.Count < count) this.pooledEntities.Enqueue(this.Instantiate());
            }

            public TEntity Spawn<TEntity>(Vector3 position, Quaternion rotation, Transform parent) where TEntity : IEntityWithoutModel
            {
                var entity = this.pooledEntities.DequeueOrDefault(this.Instantiate);
                entity.Transform.SetPositionAndRotation(position, rotation);
                entity.Transform.SetParent(parent);
                entity.gameObject.SetActive(true);
                this.spawnedEntities.Add(entity);
                this.manager.entityToPool.Add(entity, this);
                this.entityToComponents[entity].ForEach(component =>
                {
                    this.componentToTypes[component].ForEach(type => this.manager.Register(type, component));
                    component.OnSpawn();
                });
                return (TEntity)entity;
            }

            public TEntity Spawn<TEntity, TModel>(TModel model, Vector3 position, Quaternion rotation, Transform parent) where TEntity : IEntityWithModel<TModel>
            {
                var entity = this.pooledEntities.DequeueOrDefault(this.Instantiate);
                entity.Transform.SetPositionAndRotation(position, rotation);
                entity.Transform.SetParent(parent);
                entity.gameObject.SetActive(true);
                this.spawnedEntities.Add(entity);
                this.manager.entityToPool.Add(entity, this);
                ((IEntityWithModel<TModel>)entity).Model = model;
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
                entity.gameObject.SetActive(false);
                entity.Transform.SetParent(this.entitiesContainer);
                this.pooledEntities.Enqueue(entity);
            }

            public void RecycleAll()
            {
                this.spawnedEntities.SafeForEach(this.Recycle);
            }

            public void Dispose()
            {
                this.RecycleAll();
                this.pooledEntities.Clear();
                Object.Destroy(this.entitiesContainer.gameObject);
            }

            private IEntity Instantiate()
            {
                var entity = Object.Instantiate(this.prefab.gameObject, this.entitiesContainer).GetComponent<IEntity>();
                entity.gameObject.SetActive(false);
                this.entityToComponents.Add(entity, entity.GetComponentsInChildren<IComponent>());
                this.entityToComponents[entity].ForEach(component =>
                {
                    component.Manager = this.manager;
                    if (component is IHasController owner)
                    {
                        owner.Controller = this.manager.controllerFactory.Create(owner);
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