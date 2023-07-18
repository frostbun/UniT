namespace UniT.Utilities
{
    using System;

    public interface IFactory<out TProduct>
    {
        public TProduct Create();
    }

    public interface IFactory<out TProduct, in TModel>
    {
        public TProduct Create(TModel model);
    }

    public abstract class DelegateFactory<TProduct> : IFactory<TProduct>
    {
        private readonly Func<TProduct> factory;

        protected DelegateFactory(Func<TProduct> factory) => this.factory = factory;

        public TProduct Create() => this.factory();
    }

    public abstract class DelegateFactory<TProduct, TModel> : IFactory<TProduct, TModel>
    {
        private readonly Func<TModel, TProduct> factory;

        protected DelegateFactory(Func<TModel, TProduct> factory) => this.factory = factory;

        public TProduct Create(TModel model) => this.factory(model);
    }
}