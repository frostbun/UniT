namespace UniT.UI.Adapter.ABC
{
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UniT.UI.Adapter.Interfaces;
    using UnityEngine;

    public abstract class BaseNonRecyclableItemAdapter<TItem, TView, TPresenter> : MonoBehaviour
        where TView : Component, IItemView<TItem>
        where TPresenter : IItemPresenter<TItem>
    {
        private class ItemContract : IItemContract<TItem>
        {
            public TItem Item { get; set; }

            private readonly IItemView<TItem> view;

            public ItemContract(IItemView<TItem> view, IItemPresenter<TItem> presenter)
            {
                view.Contract  = presenter.Contract = this;
                view.Presenter = presenter;
                this.view      = presenter.View = view;

                this.view.Initialize();
            }

            public void Show()
            {
                this.view.Show();
                this.view.transform.SetAsLastSibling();
                this.view.gameObject.SetActive(true);
            }

            public void Hide()
            {
                this.view.gameObject.SetActive(false);
                this.view.Hide();
            }

            public void Dispose()
            {
                this.view.Dispose();
                Destroy(this.view.gameObject);
            }
        }

        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private TView itemPrefab;

        private          TItem[]                        items;
        private          IItemPresenter<TItem>.IFactory presenterFactory;
        private readonly Queue<ItemContract>            pooledContracts  = new();
        private readonly HashSet<ItemContract>          spawnedContracts = new();

        public BaseNonRecyclableItemAdapter<TItem, TView, TPresenter> Construct(IEnumerable<TItem> items, IItemPresenter<TItem>.IFactory presenterFactory = null)
        {
            this.items            = items.ToArray();
            this.presenterFactory = presenterFactory ?? IItemPresenter<TItem>.IFactory.Factory.Default();
            return this;
        }

        public void Show()
        {
            this.items.ForEach(item =>
            {
                var contract = this.pooledContracts.DequeueOrDefault(
                    () => new(
                        Instantiate(this.itemPrefab, this.content),
                        this.presenterFactory.Create(typeof(TPresenter))
                    )
                );
                this.spawnedContracts.Add(contract);
                contract.Item = item;
                contract.Show();
            });
        }

        public void Hide()
        {
        }
    }
}