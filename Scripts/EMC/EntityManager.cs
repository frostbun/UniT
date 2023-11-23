namespace UniT.EMC
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UniT.Assets;
    using UniT.EMC.Controller;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    using Object = UnityEngine.Object;

    public sealed class EntityManager : IEntityManager
    {
        #region Constructor

        private readonly IController.IFactory controllerFactory;
        private readonly IAssetsManager       assetsManager;
        private readonly ILogger              logger;

        private readonly Dictionary<string, EntityPool>     keyToPool;
        private readonly Dictionary<IEntity, EntityPool>    entityToPool;
        private readonly Dictionary<Type, HashSet<IEntity>> interfaceToEntities;

        [Preserve]
        public EntityManager(IController.IFactory controllerFactory = null, IAssetsManager assetsManager = null, ILogger logger = null)
        {
            this.controllerFactory = controllerFactory ?? IController.IFactory.Default();
            this.assetsManager     = assetsManager ?? IAssetsManager.Default();
            this.logger            = logger ?? ILogger.Default(this);

            this.keyToPool           = new();
            this.entityToPool        = new();
            this.interfaceToEntities = new();
        }

        #endregion

        #region Pooling

        UniTask IEntityManager.Load(string key, int count)
        {
            return this.keyToPool
                .GetOrAddAsync(key, () =>
                    this.assetsManager
                        .LoadComponent<IEntity>(key)
                        .ContinueWith(prefab => new EntityPool(prefab, this))
                )
                .ContinueWith(pool => pool.Load(count));
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

        void IEntityManager.RecycleAll(string key)
        {
            this.keyToPool.GetOrDefault(key)?.RecycleAll();
        }

        void IEntityManager.Unload(string key) => this.Unload(key);

        private class EntityPool
        {
            private readonly IEntity       prefab;
            private readonly EntityManager manager;

            private readonly Transform                entitiesContainer;
            private readonly Queue<IEntity>           pooledEntities;
            private readonly HashSet<IEntity>         spawnedEntities;
            private readonly ReadOnlyCollection<Type> interfaces;

            public EntityPool(IEntity prefab, EntityManager manager)
            {
                this.prefab  = prefab;
                this.manager = manager;

                prefab.gameObject.SetActive(false);
                this.entitiesContainer = new GameObject($"{prefab.gameObject.name} pool").DontDestroyOnLoad().transform;
                this.pooledEntities    = new();
                this.spawnedEntities   = new();
                this.interfaces        = prefab.GetType().GetInterfaces().AsReadOnly();
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

        #region Finalizer

        private void Unload(string key)
        {
            this.keyToPool.RemoveOrDefault(key)?.Dispose();
            this.assetsManager.Unload(key);
        }

        private void Dispose() => this.keyToPool.Keys.SafeForEach(this.Unload);

        void IDisposable.Dispose() => this.Dispose();

        ~EntityManager() => this.Dispose();

        #endregion
    }
}