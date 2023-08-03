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
                var itemView = this._pooledViews.DequeueOrDefault(() =>
                {
                    var view = Instantiate(this._itemPrefab, this._content);
                    return this.Manager.Initialize(view);
                });
                itemView.transform.SetAsLastSibling();
                itemView.gameObject.SetActive(true);
                itemView.Item = item;
                itemView.OnShow();
                this._spawnedViews.Add(itemView);
            });
            this.OnShow();
        }

        public void Hide()
        {
            this._spawnedViews.ForEach(itemView =>
            {
                itemView.gameObject.SetActive(false);
                itemView.OnHide();
                this._pooledViews.Enqueue(itemView);
            });
            this._spawnedViews.Clear();
            this.OnHide();
        }

        public void Dispose()
        {
            this.Hide();
            this._pooledViews.ForEach(itemView =>
            {
                Destroy(itemView.gameObject);
                itemView.OnDispose();
            });
            this._pooledViews.Clear();
            this.OnDispose();
        }
    }
}