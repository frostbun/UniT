namespace UniT.ECC.Component
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #endif

    public interface IComponent
    {
        public IEntityManager Manager { get; set; }

        public Transform Transform { get; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        #if UNIT_UNITASK
        public CancellationToken GetCancellationTokenOnRecycle();
        #endif
    }
}