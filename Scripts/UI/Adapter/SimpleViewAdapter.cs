namespace UniT.UI.Adapter
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.UI.View;
    using UnityEngine;

    public abstract class SimpleViewAdapter<TParams, TView> : View where TView : IViewWithParams<TParams>
    {
        [SerializeField] private Transform content;
        [SerializeField] private TView     viewPrefab;

        private readonly Queue<TView>   pooledViews  = new();
        private readonly HashSet<TView> spawnedViews = new();

        public void Set(IEnumerable<TParams> allParams)
        {
            allParams.ForEach(@params =>
            {
                var view = this.pooledViews.DequeueOrDefault(() =>
                {
                    var view = Instantiate(this.viewPrefab.gameObject, this.content).GetComponent<TView>();
                    this.Manager.Initialize(view);
                    return view;
                });
                view.Transform.SetAsLastSibling();
                view.gameObject.SetActive(true);
                this.spawnedViews.Add(view);
                view.Params = @params;
                view.OnShow();
            });
        }

        protected override void OnHide()
        {
            this.spawnedViews.Clear(view =>
            {
                view.gameObject.SetActive(false);
                view.OnHide();
                this.pooledViews.Enqueue(view);
            });
        }

        protected override void OnDispose()
        {
            this.pooledViews.Clear(view =>
            {
                view.OnDispose();
                Destroy(view.gameObject);
            });
        }
    }
}