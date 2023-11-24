namespace UniT.EMC.Model
{
    using System.Threading;
    using UnityEngine;

    public abstract class Entity<TModel> : MonoBehaviour, IEntityWithModel<TModel>
    {
        IEntityManager IEntity.Manager { get => this.Manager; set => this.Manager = value; }

        TModel IEntityWithModel<TModel>.Model { set => this.Model = value; }

        bool IEntity.IsDestroyed => !this;

        void IEntity.OnInstantiate()
        {
            this.transform = base.transform;
            this.OnInstantiate();
        }

        void IEntity.OnSpawn() => this.OnSpawn();

        void IEntity.OnRecycle()
        {
            this.recycleCts?.Cancel();
            this.recycleCts?.Dispose();
            this.recycleCts = null;
            this.OnRecycle();
        }

        public IEntityManager Manager { get; private set; }

        protected TModel Model { get; private set; }

        public new Transform transform { get; private set; }

        private CancellationTokenSource recycleCts;

        public CancellationToken GetCancellationTokenOnRecycle()
        {
            return (this.recycleCts ??= new()).Token;
        }

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
}