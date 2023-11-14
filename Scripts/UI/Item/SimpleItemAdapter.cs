namespace UniT.UI.Item
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public abstract class SimpleItemAdapter<TItem, TView> : BaseView where TView : Component, IItemView
    {
        [SerializeField] private Transform content;
        [SerializeField] private TView     viewPrefab;

        private readonly Queue<IItemView>   pooledViews  = new();
        private readonly HashSet<IItemView> spawnedViews = new();

        public void Show(IEnumerable<TItem> items)
        {
            this.Hide();
            items.ForEach(item =>
            {
                var view = this.pooledViews.DequeueOrDefault(() => this.Manager.Initialize(Instantiate(this.viewPrefab, this.content)));
                view.transform.SetAsLastSibling();
                view.gameObject.SetActive(true);
                view.Item = item;
                view.OnShow();
                this.spawnedViews.Add(view);
            });
        }

        public void Hide()
        {
            this.spawnedViews.ForEach(item =>
            {
                item.gameObject.SetActive(false);
                item.OnHide();
                this.pooledViews.Enqueue(item);
            });
            this.spawnedViews.Clear();
        }

        public void Dispose()
        {
            this.Hide();
            this.pooledViews.ForEach(item =>
            {
                item.OnDispose();
                Destroy(item.gameObject);
            });
            this.pooledViews.Clear();
        }
    }
}