namespace UniT.UI.Item.Bases
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UniT.UI.Item.Interfaces;
    using UnityEngine;

    public abstract class SimpleItemAdapter<TItem, TView> : MonoBehaviour where TView : Component, IItemView
    {
        [SerializeField]
        private RectTransform content;

        [SerializeField]
        private TView itemPrefab;

        private          IItemPresenter.Factory presenterFactory;
        private readonly Queue<IItemView>       pooledViews  = new();
        private readonly HashSet<IItemView>     spawnedViews = new();

        public void Construct(IItemPresenter.Factory presenterFactory = null)
        {
            this.presenterFactory = presenterFactory ?? IItemPresenter.Factory.Default();
        }

        public void Show(IEnumerable<TItem> items)
        {
            items.ForEach(item =>
            {
                var view = this.pooledViews.DequeueOrDefault(
                    () =>
                    {
                        var view = Instantiate(this.itemPrefab, this.content);
                        if (view is IItemViewWithPresenter viewWithPresenter)
                        {
                            var presenter = this.presenterFactory.Create(viewWithPresenter.PresenterType);
                            presenter.View              = view;
                            viewWithPresenter.Presenter = presenter;
                        }
                        view.Initialize();
                        return view;
                    });
                view.Show(item);
                this.spawnedViews.Add(view);
            });
        }

        public void Hide()
        {
            this.spawnedViews.ForEach(view =>
            {
                view.Hide();
                this.pooledViews.Enqueue(view);
            });
            this.spawnedViews.Clear();
        }

        public void Dispose()
        {
            this.Hide();
            this.pooledViews.ForEach(view => view.Dispose());
            this.pooledViews.Clear();
        }
    }
}