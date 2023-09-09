namespace UniT
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
        private readonly Func<TProduct> _factory;

        protected DelegateFactory(Func<TProduct> factory) => this._factory = factory;

        public TProduct Create() => this._factory();
    }

    public abstract class DelegateFactory<TProduct, TModel> : IFactory<TProduct, TModel>
    {
        private readonly Func<TModel, TProduct> _factory;

        protected DelegateFactory(Func<TModel, TProduct> factory) => this._factory = factory;

        public TProduct Create(TModel model) => this._factory(model);
    }
}