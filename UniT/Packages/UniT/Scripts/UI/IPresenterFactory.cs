namespace UniT.UI
{
    using System;
    using UniT.Utilities;

    public interface IPresenterFactory : IFactory<IPresenter, Type>
    {
        public static class Factory
        {
            private class PresenterFactory : IPresenterFactory
            {
                private readonly Func<Type, IPresenter> factory;

                public PresenterFactory(Func<Type, IPresenter> factory) => this.factory = factory;

                public IPresenter Create(Type type) => this.factory(type);
            }

            public static IPresenterFactory Create(Func<Type, IPresenter> factory) => new PresenterFactory(factory);
        }
    }
}