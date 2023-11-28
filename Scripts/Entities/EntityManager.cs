namespace UniT.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
    #endif

    public sealed class EntityManager : IEntityManager
    {
        #region Constructor

        private readonly IController.IFactory controllerFactory;
        private readonly IAssetsManager       assetsManager;
        private readonly ILogger              logger;

        private readonly Transform                          poolsContainer      = new GameObject(nameof(EntityManager)).DontDestroyOnLoad().transform;
        private readonly Dictionary<IEntity, EntityPool>    prefabToPool        = new();
        private readonly Dictionary<string, EntityPool>     keyToPool           = new();
        private readonly Dictionary<IEntity, EntityPool>    entityToPool        = new();
        private readonly Dictionary<Type, HashSet<IEntity>> interfaceToEntities = new();

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
            var isLoaded = this.prefabToPool.TryAdd(prefab, () => new(prefab, this));
            this.logger.Debug(isLoaded ? $"Loaded {prefab.gameObject.name} pool" : $"Using cached {prefab.gameObject.name} pool");
            this.prefabToPool[prefab].Load(count);
        }

        void IEntityManager.Load(string key, int count)
        {
            var isLoaded = this.keyToPool.TryAdd(key, () => new(this.assetsManager.LoadComponent<IEntity>(key), this));
            this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
            this.keyToPool[key].Load(count);
        }

        #if UNIT_UNITASK
        async UniTask IEntityManager.LoadAsync(string key, int count, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var isLoaded = await this.keyToPool.TryAddAsync(key, async () => new(await this.assetsManager.LoadComponentAsync<IEntity>(key, progress, cancellationToken), this));
            this.logger.Debug(isLoaded ? $"Loaded {key} pool" : $"Using cached {key} pool");
            this.keyToPool[key].Load(count);
        }
        #endif

        TEntity IEntityManager.Spawn<TEntity>(TEntity prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var entity = (TEntity)this.prefabToPool[prefab].Spawn(position, rotation, parent);
            entity.OnSpawn();
            return entity;
        }

        TEntity IEntityManager.Spawn<TEntity, TModel>(TEntity prefab, TModel model, Vector3 position, Quaternion rotation, Transform parent)
        {
            var entity = (TEntity)this.prefabToPool[prefab].Spawn(position, rotation, parent);
            entity.Model = model;
            entity.OnSpawn();
            return entity;
        }

        TEntity IEntityManager.Spawn<TEntity>(string key, Vector3 position, Quaternion rotation, Transform parent)
        {
            var entity = (TEntity)this.keyToPool[key].Spawn(position, rotation, parent);
            entity.OnSpawn();
            return entity;
        }

        TEntity IEntityManager.Spawn<TEntity, TModel>(string key, TModel model, Vector3 position, Quaternion rotation, Transform parent)
        {
            var entity = (TEntity)this.keyToPool[key].Spawn(position, rotation, parent);
            entity.Model = model;
            entity.OnSpawn();
            return entity;
        }

        void IEntityManager.Recycle(IEntity entity)
        {
            this.entityToPool[entity].Recycle(entity);
        }

        void IEntityManager.RecycleAll(IEntity prefab)
        {
            this.prefabToPool.GetOrDefault(prefab)?.RecycleAll();
        }

        void IEntityManager.RecycleAll(string key)
        {
            this.keyToPool.GetOrDefault(key)?.RecycleAll();
        }

        void IEntityManager.Unload(IEntity prefab)
        {
            this.prefabToPool.RemoveOrDefault(prefab).Dispose();
        }

        void IEntityManager.Unload(string key)
        {
            this.keyToPool.RemoveOrDefault(key).Dispose();
            this.assetsManager.Unload(key);
        }

        private class EntityPool
        {
            private readonly IEntity                  prefab;
            private readonly EntityManager            manager;
            private readonly Transform                entitiesContainer;
            private readonly ReadOnlyCollection<Type> interfaces;

            private readonly Queue<IEntity>   pooledEntities  = new();
            private readonly HashSet<IEntity> spawnedEntities = new();

            public EntityPool(IEntity prefab, EntityManager manager)
            {
                this.prefab  = prefab;
                this.manager = manager;
                prefab.gameObject.SetActive(false);
                this.entitiesContainer = new GameObject($"{prefab.gameObject.name} pool").transform;
                this.entitiesContainer.SetParent(manager.poolsContainer);
                this.interfaces = prefab.GetType().GetInterfaces().AsReadOnly();
            }

            public void Load(int count)
            {
                while (this.pooledEntities.Count < count) this.pooledEntities.Enqueue(this.Instantiate());
            }

            public IEntity Spawn(Vector3 position, Quaternion rotation, Transform parent)
            {
                var entity = this.pooledEntities.DequeueOrDefault(this.Instantiate);
                entity.transform.SetPositionAndRotation(position, rotation);
                entity.transform.SetParent(parent);
                entity.gameObject.SetActive(true);
                this.spawnedEntities.Add(entity);
                this.manager.entityToPool.Add(entity, this);
                this.interfaces.ForEach(@interface => this.manager.Register(@interface, entity));
                return entity;
            }

            public void Recycle(IEntity entity)
            {
                entity.OnRecycle();
                this.interfaces.ForEach(@interface => this.manager.Unregister(@interface, entity));
                this.manager.entityToPool.Remove(entity);
                this.spawnedEntities.Remove(entity);
                if (entity.IsDestroyed) return;
                entity.gameObject.SetActive(false);
                entity.transform.SetParent(this.entitiesContainer);
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
                if (!this.entitiesContainer) return;
                Object.Destroy(this.entitiesContainer.gameObject);
            }

            private IEntity Instantiate()
            {
                var entity = Object.Instantiate(this.prefab.gameObject, this.entitiesContainer).GetComponent<IEntity>();
                entity.Manager = this.manager;
                entity.GetComponentsInChildren<IHasController>().ForEach(component =>
                {
                    component.Controller = this.manager.controllerFactory.Create(component);
                });
                entity.OnInstantiate();
                return entity;
            }
        }

        #endregion

        #region Query

        IEnumerable<T> IEntityManager.Query<T>()
        {
            return this.GetCache(typeof(T)).Cast<T>();
        }

        private void Register(Type @interface, IEntity entity)
        {
            this.GetCache(@interface).Add(entity);
        }

        private void Unregister(Type @interface, IEntity entity)
        {
            this.GetCache(@interface).Remove(entity);
        }

        private HashSet<IEntity> GetCache(Type @interface)
        {
            return this.interfaceToEntities.GetOrAdd(@interface, () => new());
        }

        #endregion
    }
}