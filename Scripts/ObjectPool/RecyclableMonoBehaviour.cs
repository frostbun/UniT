namespace UniT.ObjectPool
{
    using UnityEngine;

    public abstract class RecyclableMonoBehaviour : MonoBehaviour, IRecyclable
    {
        public IObjectPoolManager Manager { get; private set; }

        protected virtual void OnInstantiate()
        {
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnRecycle()
        {
        }

        #region Interface Implementation

        IObjectPoolManager IRecyclable.Manager { get => this.Manager; set => this.Manager = value; }

        void IRecyclable.OnInstantiate() => this.OnInstantiate();

        void IRecyclable.OnSpawn() => this.OnSpawn();

        void IRecyclable.OnRecycle() => this.OnRecycle();

        #endregion
    }
}