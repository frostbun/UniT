namespace UniT.Entities
{
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif
    using System;
    using System.Collections.Generic;
    using UniT.Logging;
    using UnityEngine;

    public interface IEntityManager : IHasLogger, IDisposable
    {
        public void Load(IEntity prefab, int count = 1);

        public void Load(string key, int count = 1);

        public TEntity Spawn<TEntity>(TEntity prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel;

        public TEntity Spawn<TEntity, TModel>(TEntity prefab, TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel>;

        public TEntity Spawn<TEntity>(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel;

        public TEntity Spawn<TEntity, TModel>(string key, TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel>;

        public void Recycle(IEntity instance);

        public void RecycleAll(IEntity prefab);

        public void RecycleAll(string key);

        public void Unload(IEntity prefab);

        public void Unload(string key);

        public IEnumerable<T> Query<T>();

        #region Implicit Key

        #if UNITY_2021_2_OR_NEWER
        public void Load<TEntity>(int count = 1) where TEntity : IEntity => this.Load(typeof(TEntity).GetKey(), count);

        public TEntity Spawn<TEntity>(Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel => this.Spawn<TEntity>(typeof(TEntity).GetKey(), position, rotation, parent);

        public TEntity Spawn<TEntity, TModel>(TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel> => this.Spawn<TEntity, TModel>(typeof(TEntity).GetKey(), model, position, rotation, parent);

        public void RecycleAll<TEntity>() where TEntity : IEntity => this.RecycleAll(typeof(TEntity).GetKey());

        public void Unload<TEntity>() where TEntity : IEntity => this.Unload(typeof(TEntity).GetKey());
        #endif

        #endregion

        #region Async

        #if UNIT_UNITASK
        public UniTask LoadAsync(string key, int count = 1, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        #if UNITY_2021_2_OR_NEWER
        public UniTask LoadAsync<TEntity>(int count = 1, IProgress<float> progress = null, CancellationToken cancellationToken = default) where TEntity : IEntity => this.LoadAsync(typeof(TEntity).GetKey(), count, progress, cancellationToken);
        #endif
        #else
        public IEnumerator LoadAsync(string key, int count = 1, Action callback = null, IProgress<float> progress = null);

        #if UNITY_2021_2_OR_NEWER
        public IEnumerator LoadAsync<TEntity>(int count = 1, Action callback = null, IProgress<float> progress = null) where TEntity : IEntity => this.LoadAsync(typeof(TEntity).GetKey(), count, callback, progress);
        #endif
        #endif

        #endregion
    }
}