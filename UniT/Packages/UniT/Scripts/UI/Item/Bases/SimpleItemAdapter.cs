namespace UniT.UI.Item.Bases
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;

    public abstract class SimpleItemAdapter<TItem, TView, TPresenter> : MonoBehaviour
        where TView : Component, IItemView
        where TPresenter : IItemPresenter
    {
        private class ItemContract
        {
            private readonly IItemView view;

            public ItemContract(IItemView view, IItemPresenter presenter)
            {
                view.Presenter = presenter;
                this.view      = presenter.View = view;
                this.view.Initialize();
            }

            public void Show(TItem item)
            {
                this.view.Item = item;
                this.view.Show();
                this.view.transform.SetAsLastSibling();
                this.view.gameObject.SetActive(true);
            }

            public void Hide()
            {
                this.view.Hide();
                this.view.gameObject.SetActive(false);
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

        private          IItemPresenter.Factory presenterFactory;
        private readonly Queue<ItemContract>    pooledContracts  = new();
        private readonly HashSet<ItemContract>  spawnedContracts = new();

        public SimpleItemAdapter<TItem, TView, TPresenter> Construct(IItemPresenter.Factory presenterFactory = null)
        {
            this.presenterFactory = presenterFactory ?? IItemPresenter.Factory.Default();
            return this;
        }

        public void Show(IEnumerable<TItem> items)
        {
            items.ForEach(item =>
            {
                var contract = this.pooledContracts.DequeueOrDefault(
                    () => new(
                        Instantiate(this.itemPrefab, this.content),
                        this.presenterFactory.Create(typeof(TPresenter))
                    )
                );
                contract.Show(item);
                this.spawnedContracts.Add(contract);
            });
        }

        public void Hide()
        {
            this.spawnedContracts.ForEach(contract =>
            {
                contract.Hide();
                this.pooledContracts.Enqueue(contract);
            });
            this.spawnedContracts.Clear();
        }

        public void Dispose()
        {
            this.Hide();
            this.pooledContracts.ForEach(contract => contract.Dispose());
            this.pooledContracts.Clear();
        }
    }

    public abstract class SimpleItemAdapter<TItem, TView> : SimpleItemAdapter<TItem, TView, NoItemPresenter>
        where TView : Component, IItemView
    {
    }
}