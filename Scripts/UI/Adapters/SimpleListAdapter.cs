namespace UniT.UI.Adapters
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.UI.View;
    using UnityEngine;

    public abstract class SimpleListAdapter<TParams, TView> : View where TView : IViewWithParams<TParams>
    {
        [SerializeField] private Transform content;
        [SerializeField] private TView     prefab;

        private readonly Queue<TView>   pooledViews  = new Queue<TView>();
        private readonly HashSet<TView> spawnedViews = new HashSet<TView>();

        public void Set(IEnumerable<TParams> allParams)
        {
            allParams.ForEach(@params =>
            {
                var view = this.pooledViews.DequeueOrDefault(() =>
                {
                    var view = Instantiate(this.prefab.GameObject, this.content).GetComponent<TView>();
                    this.Manager.Initialize(view, this.Activity);
                    return view;
                });
                view.Transform.SetAsLastSibling();
                view.GameObject.SetActive(true);
                this.spawnedViews.Add(view);
                view.Params = @params;
                view.OnShow();
            });
        }

        protected override void OnHide()
        {
            this.spawnedViews.Clear(view =>
            {
                view.OnHide();
                view.GameObject.SetActive(false);
                this.pooledViews.Enqueue(view);
            });
        }

        protected override void OnDispose()
        {
            this.pooledViews.Clear(view =>
            {
                view.OnDispose();
                Destroy(view.GameObject);
            });
        }
    }
}