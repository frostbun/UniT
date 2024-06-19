#nullable enable
namespace UniT.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.DI;
    using UniT.Entities.Component;
    using UniT.Entities.Controller;
    using UniT.Entities.Entity;
    using UniT.Extensions;
    using UniT.Logging;
    using UniT.Pooling;
    using UnityEngine;
    using UnityEngine.Scripting;
    using ILogger = UniT.Logging.ILogger;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class EntityManager : IEntityManager
    {
        #region Constructor

        private readonly IDependencyContainer container;
        private readonly IObjectPoolManager   objectPoolManager;
        private readonly ILogger              logger;

        private readonly Dictionary<IEntity, IComponent[]>     entities         = new Dictionary<IEntity, IComponent[]>();
        private readonly Dictionary<IComponent, Type[]>        componentToTypes = new Dictionary<IComponent, Type[]>();
        private readonly Dictionary<Type, HashSet<IComponent>> typeToComponents = new Dictionary<Type, HashSet<IComponent>>();
        private readonly Dictionary<IEntity, object>           spawnedEntities  = new Dictionary<IEntity, object>();

        [Preserve]
        public EntityManager(IDependencyContainer container, IObjectPoolManager objectPoolManager, ILoggerManager loggerManager)
        {
            this.container         = container;
            this.objectPoolManager = objectPoolManager;
            this.logger            = loggerManager.GetLogger(this);
            this.logger.Debug("Constructed");
        }

        #endregion

        void IEntityManager.Load(IEntity prefab, int count) => this.objectPoolManager.Load(prefab.GameObject, count);

        void IEntityManager.Load(string key, int count) => this.objectPoolManager.Load(key, count);

        #if UNIT_UNITASK
        UniTask IEntityManager.LoadAsync(string key, int count, IProgress<float>? progress, CancellationToken cancellationToken) => this.objectPoolManager.LoadAsync(key, count, progress, cancellationToken);
        #else
        IEnumerator IEntityManager.LoadAsync(string key, int count, Action? callback, IProgress<float>? progress) => this.objectPoolManager.LoadAsync(key, count, callback, progress);
        #endif

        TEntity IEntityManager.Spawn<TEntity>(TEntity prefab, Vector3 position, Quaternion rotation, Transform? parent)
        {
            var entity = this.objectPoolManager.Spawn(prefab.GameObject, position, rotation, parent).GetComponent<TEntity>();
            this.OnSpawn(entity);
            this.spawnedEntities.Add(entity, prefab);
            return entity;
        }

        TEntity IEntityManager.Spawn<TEntity, TParams>(TEntity prefab, TParams @params, Vector3 position, Quaternion rotation, Transform? parent)
        {
            var entity = this.objectPoolManager.Spawn(prefab.GameObject, position, rotation, parent).GetComponent<TEntity>();
            entity.Params = @params;
            this.OnSpawn(entity);
            this.spawnedEntities.Add(entity, prefab);
            return entity;
        }

        TEntity IEntityManager.Spawn<TEntity>(string key, Vector3 position, Quaternion rotation, Transform? parent)
        {
            var entity = this.objectPoolManager.Spawn(key, position, rotation, parent).GetComponentOrThrow<TEntity>();
            this.OnSpawn(entity);
            this.spawnedEntities.Add(entity, key);
            return entity;
        }

        TEntity IEntityManager.Spawn<TEntity, TParams>(string key, TParams @params, Vector3 position, Quaternion rotation, Transform? parent)
        {
            var entity = this.objectPoolManager.Spawn(key, position, rotation, parent).GetComponentOrThrow<TEntity>();
            entity.Params = @params;
            this.OnSpawn(entity);
            this.spawnedEntities.Add(entity, key);
            return entity;
        }

        void IEntityManager.Recycle(IEntity entity)
        {
            this.spawnedEntities.Remove(entity);
            this.OnRecycle(entity);
            this.objectPoolManager.Recycle(entity.GameObject);
        }

        void IEntityManager.RecycleAll(IEntity prefab)
        {
            this.OnRecycleAll(prefab);
            this.objectPoolManager.RecycleAll(prefab.GameObject);
        }

        void IEntityManager.RecycleAll(string key)
        {
            this.OnRecycleAll(key);
            this.objectPoolManager.RecycleAll(key);
        }

        void IEntityManager.Cleanup(IEntity prefab, int retainCount)
        {
            this.objectPoolManager.Cleanup(prefab.GameObject, retainCount);
            this.OnCleanup();
        }

        void IEntityManager.Cleanup(string key, int retainCount)
        {
            this.objectPoolManager.Cleanup(key, retainCount);
            this.OnCleanup();
        }

        void IEntityManager.Unload(IEntity prefab)
        {
            this.OnRecycleAll(prefab);
            this.objectPoolManager.RecycleAll(prefab.GameObject);
            this.objectPoolManager.Unload(prefab.GameObject);
            this.OnCleanup();
        }

        void IEntityManager.Unload(string key)
        {
            this.OnRecycleAll(key);
            this.objectPoolManager.RecycleAll(key);
            this.objectPoolManager.Unload(key);
            this.OnCleanup();
        }

        private void OnSpawn(IEntity entity)
        {
            this.entities.GetOrAdd(entity, () =>
            {
                var components = entity.GetComponentsInChildren<IComponent>();
                components.ForEach(component =>
                {
                    this.componentToTypes.Add(
                        component,
                        component.GetType()
                            .GetInterfaces()
                            .Where(type => !typeof(IComponent).IsAssignableFrom(type))
                            .Where(type => !typeof(IHasController).IsAssignableFrom(type))
                            .Prepend(component.GetType())
                            .ToArray()
                    );
                    component.Manager = this;
                    component.Entity  = entity;
                    if (component is IHasController owner)
                    {
                        var controller = (IController)this.container.Instantiate(owner.ControllerType);
                        controller.Owner = owner;
                        owner.Controller = controller;
                    }
                    component.OnInstantiate();
                });
                return components;
            }).ForEach(component =>
            {
                this.componentToTypes[component].ForEach(type => this.typeToComponents.GetOrAdd(type).Add(component));
                component.OnSpawn();
            });
        }

        private void OnRecycle(IEntity entity)
        {
            this.entities[entity].ForEach(component =>
            {
                component.OnRecycle();
                this.componentToTypes[component].ForEach(type => this.typeToComponents.GetOrAdd(type).Remove(component));
            });
        }

        private void OnRecycleAll(object obj)
        {
            this.spawnedEntities.RemoveAll((entity, key) =>
            {
                if (key != obj) return false;
                this.OnRecycle(entity);
                return true;
            });
        }

        private void OnCleanup()
        {
            this.entities.RemoveAll((entity, components) =>
            {
                if (entity.GameObject) return false;
                components.ForEach(component => this.componentToTypes.Remove(component));
                return true;
            });
        }

        IEnumerable<T> IEntityManager.Query<T>()
        {
            return this.typeToComponents.GetOrDefault(typeof(T))?.Cast<T>() ?? Enumerable.Empty<T>();
        }
    }
}