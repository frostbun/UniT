#nullable enable
namespace UniT.Entities.Component
{
    using UniT.Entities.Entity;
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public interface IComponent
    {
        public IEntityManager Manager { get; set; }

        public IEntity Entity { get; set; }

        public string Name { get; }

        public GameObject GameObject { get; }

        public Transform Transform { get; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        #if UNIT_UNITASK
        public CancellationToken GetCancellationTokenOnRecycle();
        #else
        public void StartCoroutine(IEnumerator coroutine);

        public void StopCoroutine(IEnumerator coroutine);

        public IEnumerator GatherCoroutines(params IEnumerator[] coroutines);

        public IEnumerator GatherCoroutines(IEnumerable<IEnumerator> coroutines);
        #endif
    }
}