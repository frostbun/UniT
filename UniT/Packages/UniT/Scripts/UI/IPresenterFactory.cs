namespace UniT.UI
{
    using System;

    public interface IPresenterFactory
    {
        private class PresenterFactory : IPresenterFactory
        {
            private readonly Func<Type, IPresenter> factory;

            public PresenterFactory(Func<Type, IPresenter> factory) => this.factory = factory;

            public IPresenter Create(Type type) => this.factory(type);
        }

        public static IPresenterFactory CreateFactory(Func<Type, IPresenter> factory) => new PresenterFactory(factory);

        public IPresenter Create(Type type);
    }
}