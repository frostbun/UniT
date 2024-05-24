#if !UNIT_UNITASK
#nullable enable
namespace UniT.Utilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public static class CoroutineRunner
    {
        private static readonly _ Runner = new GameObject(nameof(CoroutineRunner)).AddComponent<_>().DontDestroyOnLoad();

        public static void Start(this IEnumerator coroutine) => Runner.StartCoroutine(coroutine);

        public static void Stop(this IEnumerator coroutine) => Runner.StopCoroutine(coroutine);

        public static IEnumerator Gather(this IEnumerable<IEnumerator> coroutines) => Runner.GatherCoroutines(coroutines);

        public static IEnumerator Gather(params IEnumerator[] coroutines) => Runner.GatherCoroutines(coroutines);

        private class _ : BetterMonoBehavior
        {
        }
    }
}
#endif