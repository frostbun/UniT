namespace UniT.UI.UIElement
{
    using UnityEngine;
    #if UNIT_UNITASK
    using System.Threading;
    #else
    using System.Collections;
    using System.Collections.Generic;
    #endif

    public interface IUIElement
    {
        public IUIManager Manager { get; set; }

        public RectTransform Transform { get; }

        public GameObject gameObject { get; }

        public void OnInitialize();

        public void OnShow();

        public void OnHide();

        public void OnDispose();

        #if UNIT_UNITASK
        public CancellationToken GetCancellationTokenOnHide();
        #else
        public void StartCoroutine(IEnumerator coroutine);

        public void StopCoroutine(IEnumerator coroutine);

        public IEnumerator GatherCoroutines(params IEnumerator[] coroutines);

        public IEnumerator GatherCoroutines(IEnumerable<IEnumerator> coroutines);
        #endif
    }

    public interface IUIElementWithoutParams : IUIElement
    {
    }

    public interface IUIElementWithParams<TParams> : IUIElement
    {
        public TParams Params { get; set; }
    }
}