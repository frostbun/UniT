#nullable enable
namespace UniT.Utilities
{
    using UnityEngine;
    #if !UNIT_UNITASK
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    #endif

    public abstract class BetterMonoBehavior : MonoBehaviour
    {
        #if !UNIT_UNITASK
        private readonly HashSet<IEnumerator> runningCoroutines = new HashSet<IEnumerator>();

        public new void StartCoroutine(IEnumerator coroutine)
        {
            this.runningCoroutines.Add(coroutine);
            base.StartCoroutine(coroutine.Finally(() => this.runningCoroutines.Remove(coroutine)));
        }

        public new void StopCoroutine(IEnumerator coroutine)
        {
            base.StopCoroutine(coroutine);
            this.runningCoroutines.Remove(coroutine);
            (coroutine as IDisposable)?.Dispose();
        }

        public IEnumerator GatherCoroutines(params IEnumerator[] coroutines)
        {
            try
            {
                var count = coroutines.Length;
                coroutines.ForEach(coroutine => this.StartCoroutine(coroutine.Then(() => --count)));
                yield return new WaitUntil(() => count is 0);
            }
            finally
            {
                coroutines.ForEach(this.StopCoroutine);
            }
        }

        public IEnumerator GatherCoroutines(IEnumerable<IEnumerator> coroutines)
        {
            return this.GatherCoroutines(coroutines.ToArray());
        }

        protected virtual void OnDisable()
        {
            this.runningCoroutines.SafeForEach(this.StopCoroutine);
        }
        #endif
    }
}