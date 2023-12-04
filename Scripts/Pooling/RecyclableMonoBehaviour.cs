namespace UniT.Pooling
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public abstract class RecyclableMonoBehaviour : MonoBehaviour, IRecyclable
    {
        IObjectPoolManager IRecyclable.Manager { get => this.Manager; set => this.Manager = value; }

        void IRecyclable.OnInstantiate()
        {
            this.transform = base.transform;
            this.OnInstantiate();
        }

        void IRecyclable.OnSpawn() => this.OnSpawn();

        void IRecyclable.OnRecycle()
        {
            #if UNIT_UNITASK
            this.recycleCts?.Cancel();
            this.recycleCts?.Dispose();
            this.recycleCts = null;
            #endif
            this.OnRecycle();
        }

        protected IObjectPoolManager Manager { get; private set; }

        public new Transform transform { get; private set; }

        #if UNIT_UNITASK
        private CancellationTokenSource recycleCts;

        protected CancellationToken GetCancellationTokenOnRecycle()
        {
            return (this.recycleCts ??= new CancellationTokenSource()).Token;
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
}