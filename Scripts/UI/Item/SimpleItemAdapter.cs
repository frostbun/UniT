namespace UniT.UI.Item
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public abstract class SimpleItemAdapter<TItem, TView> : BaseView where TView : Component, IItemView
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private TView         _itemPrefab;

        private readonly Queue<IItemView>   _pooledViews  = new();
        private readonly HashSet<IItemView> _spawnedViews = new();

        public void Show(IEnumerable<TItem> items)
        {
            this.Hide();
            items.ForEach(item =>
            {
                var itemView = this._pooledViews.DequeueOrDefault(() => this.Manager.Initialize(Instantiate(this._itemPrefab, this._content)));
                itemView.Transform.SetAsLastSibling();
                itemView.GameObject.SetActive(true);
                itemView.Item = item;
                itemView.OnShow();
                this._spawnedViews.Add(itemView);
            });
        }

        public void Hide()
        {
            this._spawnedViews.ForEach(itemView =>
            {
                itemView.GameObject.SetActive(false);
                itemView.OnHide();
                this._pooledViews.Enqueue(itemView);
            });
            this._spawnedViews.Clear();
        }

        public void Dispose()
        {
            this.Hide();
            this._pooledViews.ForEach(itemView =>
            {
                itemView.OnDispose();
                Destroy(itemView.GameObject);
            });
            this._pooledViews.Clear();
        }
    }
}