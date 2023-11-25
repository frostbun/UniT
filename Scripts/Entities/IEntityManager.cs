namespace UniT.Entities
{
    using System;
    using System.Collections.Generic;
    using UniT.Entities.Model;
    using UniT.Extensions;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public interface IEntityManager : IDisposable
    {
        public void Load(string key, int count = 1);

        #if UNIT_UNITASK
        public UniTask LoadAsync(string key, int count = 1, IProgress<float> progress = null, CancellationToken cancellationToken = default);
        #endif

        public TEntity Spawn<TEntity>(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel;

        public TEntity Spawn<TEntity, TModel>(string key, TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel>;

        #if UNIT_UNITASK
        public CancellationToken GetCancellationTokenOnRecycle(IEntity entity);
        #endif

        public void Recycle(IEntity entity);

        public void RecycleAll(string key);

        public void Unload(string key);

        public IEnumerable<T> Query<T>();

        #region Implicit Key

        public void Load<TEntity>(int count = 1) where TEntity : IEntity => this.Load(typeof(TEntity).GetKey(), count);

        #if UNIT_UNITASK
        public UniTask LoadAsync<TEntity>(int count = 1, IProgress<float> progress = null, CancellationToken cancellationToken = default) where TEntity : IEntity => this.LoadAsync(typeof(TEntity).GetKey(), count, progress, cancellationToken);
        #endif

        public TEntity Spawn<TEntity>(Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel => this.Spawn<TEntity>(typeof(TEntity).GetKey(), position, rotation, parent);

        public TEntity Spawn<TEntity, TModel>(TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel> => this.Spawn<TEntity, TModel>(typeof(TEntity).GetKey(), model, position, rotation, parent);

        public void RecycleAll<TEntity>() where TEntity : IEntity => this.RecycleAll(typeof(TEntity).GetKey());

        public void Unload<TEntity>() where TEntity : IEntity => this.Unload(typeof(TEntity).GetKey());

        #endregion
    }
}