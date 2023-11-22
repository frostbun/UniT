namespace UniT.EMC
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UniT.EMC.Model;
    using UniT;
    using UniT.Extensions;
    using UnityEngine;

    public interface IEntityManager : IDisposable
    {
        public UniTask Load(string key, int count = 1);

        public TEntity Spawn<TEntity>(string key, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel;

        public TEntity Spawn<TEntity, TModel>(string key, TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel>;

        public void Recycle(IEntity entity);

        public void RecycleAll(string key);

        public void Unload(string key);

        public IEnumerable<T> Query<T>();

        #region Implicit Key

        public UniTask Load<TEntity>(int count = 1) where TEntity : IEntity => this.Load(typeof(TEntity).GetKey(), count);

        public TEntity Spawn<TEntity>(Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithoutModel => this.Spawn<TEntity>(typeof(TEntity).GetKey(), position, rotation, parent);

        public TEntity Spawn<TEntity, TModel>(TModel model, Vector3 position = default, Quaternion rotation = default, Transform parent = null) where TEntity : IEntityWithModel<TModel> => this.Spawn<TEntity, TModel>(typeof(TEntity).GetKey(), model, position, rotation, parent);

        public void RecycleAll<TEntity>() where TEntity : IEntity => this.RecycleAll(typeof(TEntity).GetKey());

        public void Unload<TEntity>() where TEntity : IEntity => this.Unload(typeof(TEntity).GetKey());

        #endregion
    }
}