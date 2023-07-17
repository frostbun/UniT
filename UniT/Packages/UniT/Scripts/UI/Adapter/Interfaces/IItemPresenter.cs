namespace UniT.UI.Adapter.Interfaces
{
    using System;

    public interface IItemPresenter<TItem>
    {
        public interface IFactory
        {
            private class PresenterFactory : IFactory
            {
                private readonly Func<Type, IItemPresenter<TItem>> factory;

                public PresenterFactory(Func<Type, IItemPresenter<TItem>> factory) => this.factory = factory;

                public IItemPresenter<TItem> Create(Type type) => this.factory(type);
            }

            public static class Factory
            {
                public static Func<IFactory> Default { get; set; } = () => new PresenterFactory(type => (IItemPresenter<TItem>)Activator.CreateInstance(type));

                public static IFactory Create(Func<Type, IItemPresenter<TItem>> factory) => new PresenterFactory(factory);
            }

            public IItemPresenter<TItem> Create(Type type);
        }

        protected internal IItemContract<TItem> Contract { set; }

        protected internal IItemView<TItem> View { set; }
    }
}