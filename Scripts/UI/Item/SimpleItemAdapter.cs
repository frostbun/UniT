namespace UniT.UI.Item
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public abstract class SimpleItemAdapter<TItem, TView> : BaseView where TView : Component, IItemView
    {
        [SerializeField] private Transform _content;
        [SerializeField] private TView     _viewPrefab;

        private readonly Queue<IItemView>   _pooledViews  = new();
        private readonly HashSet<IItemView> _spawnedViews = new();

        public void Show(IEnumerable<TItem> items)
        {
            this.Hide();
            items.ForEach(item =>
            {
                var view = this._pooledViews.DequeueOrDefault(() => this.Manager.Initialize(Instantiate(this._viewPrefab, this._content)));
                view.transform.SetAsLastSibling();
                view.gameObject.SetActive(true);
                view.Item = item;
                view.OnShow();
                this._spawnedViews.Add(view);
            });
        }

        public void Hide()
        {
            this._spawnedViews.ForEach(item =>
            {
                item.gameObject.SetActive(false);
                item.OnHide();
                this._pooledViews.Enqueue(item);
            });
            this._spawnedViews.Clear();
        }

        public void Dispose()
        {
            this.Hide();
            this._pooledViews.ForEach(item =>
            {
                item.OnDispose();
                Destroy(item.gameObject);
            });
            this._pooledViews.Clear();
        }
    }
}