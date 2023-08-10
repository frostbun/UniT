namespace UniT.UI.Item
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine;

    public abstract class SimpleItemAdapter<TModel, TView> : BaseView where TView : Component, IItemView
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private TView         _itemPrefab;

        private readonly Queue<IItemView>   _pooledItems  = new();
        private readonly HashSet<IItemView> _spawnedItems = new();

        public void Show(IEnumerable<TModel> models)
        {
            this.Hide();
            models.ForEach(model =>
            {
                var item = this._pooledItems.DequeueOrDefault(() => this.Manager.Initialize(Instantiate(this._itemPrefab, this._content)));
                item.Transform.SetAsLastSibling();
                item.GameObject.SetActive(true);
                item.Model = model;
                item.OnShow();
                this._spawnedItems.Add(item);
            });
        }

        public void Hide()
        {
            this._spawnedItems.ForEach(item =>
            {
                item.GameObject.SetActive(false);
                item.OnHide();
                this._pooledItems.Enqueue(item);
            });
            this._spawnedItems.Clear();
        }

        public void Dispose()
        {
            this.Hide();
            this._pooledItems.ForEach(item =>
            {
                item.OnDispose();
                Destroy(item.GameObject);
            });
            this._pooledItems.Clear();
        }
    }
}