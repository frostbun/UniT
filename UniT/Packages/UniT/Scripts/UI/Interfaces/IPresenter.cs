namespace UniT.UI.Interfaces
{
    using System;

    public interface IPresenter
    {
        public interface IFactory
        {
            private class PresenterFactory : IFactory
            {
                private readonly Func<Type, IPresenter> factory;

                public PresenterFactory(Func<Type, IPresenter> factory) => this.factory = factory;

                public IPresenter Create(Type type) => this.factory(type);
            }

            public static class Factory
            {
                public static Func<IFactory> Default { get; set; } = () => new PresenterFactory(type => (IPresenter)Activator.CreateInstance(type));

                public static IFactory Create(Func<Type, IPresenter> factory) => new PresenterFactory(factory);
            }

            public IPresenter Create(Type type);
        }

        protected internal IContract Contract { set; }

        protected internal IView View { set; }
    }
}