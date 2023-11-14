namespace UniT.ObjectPool
{
    using System.Threading;
    using UnityEngine;

    public abstract class RecyclableMonoBehaviour : MonoBehaviour, IRecyclable
    {
        IObjectPoolManager IRecyclable.Manager { get => this.Manager; set => this.Manager = value; }

        void IRecyclable.OnInstantiate() => this.OnInstantiate();

        void IRecyclable.OnSpawn() => this.OnSpawn();

        void IRecyclable.OnRecycle()
        {
            this.recycleCts?.Cancel();
            this.recycleCts?.Dispose();
            this.recycleCts = null;
            this.OnRecycle();
        }

        public IObjectPoolManager Manager { get; private set; }

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