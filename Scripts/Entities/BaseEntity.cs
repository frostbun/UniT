namespace UniT.Entities
{
    using UnityEngine;

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

        void IEntity.OnRecycle() => this.OnRecycle();

        protected IEntityManager Manager { get; private set; }

        public new Transform transform { get; private set; }

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