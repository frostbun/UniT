namespace UniT.Entities
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class BaseEntity : MonoBehaviour, IEntity
    {
        IEntityManager IEntity.Manager { get => this.Manager; set => this.Manager = value; }

        bool IEntity.IsDestroyed => !this;

        void IEntity.OnInstantiate()
        {
            this.transform = base.transform;
            this.OnInstantiate();
        }

        void IEntity.OnSpawn() => this.OnSpawn();

        void IEntity.OnRecycle()
        {
            #if UNIT_UNITASK
            this.hideCts?.Cancel();
            this.hideCts?.Dispose();
            this.hideCts = null;
            #endif
            this.OnRecycle();
        }

        protected IEntityManager Manager { get; private set; }

        public new Transform transform { get; private set; }

        #if UNIT_UNITASK
        private CancellationTokenSource hideCts;

        protected CancellationToken GetCancellationTokenOnHide()
        {
            return (this.hideCts ??= new()).Token;
        }
        #endif

        protected virtual void OnInstantiate()
        {
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnRecycle()
        {
        }
    }

    public abstract class Entity : BaseEntity, IEntityWithoutModel
    {
    }

    public abstract class Entity<TModel> : BaseEntity, IEntityWithModel<TModel>
    {
        TModel IEntityWithModel<TModel>.Model { set => this.Model = value; }

        protected TModel Model { get; private set; }
    }
}