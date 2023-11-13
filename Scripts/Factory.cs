namespace UniT
{
    using System;
    using Cysharp.Threading.Tasks;

    public interface IFactory<out TProduct>
    {
        public TProduct Create();
    }

    public interface IFactory<out TProduct, in TModel>
    {
        public TProduct Create(TModel model);
    }

    public interface IAsyncFactory<TProduct> : IFactory<UniTask<TProduct>>
    {
    }

    public interface IAsyncFactory<TProduct, in TModel> : IFactory<UniTask<TProduct>, TModel>
    {
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

    public abstract class AsyncDelegateFactory<TProduct> : DelegateFactory<UniTask<TProduct>>, IAsyncFactory<TProduct>
    {
        protected AsyncDelegateFactory(Func<UniTask<TProduct>> factory) : base(factory)
        {
        }
    }

    public abstract class AsyncDelegateFactory<TProduct, TModel> : DelegateFactory<UniTask<TProduct>, TModel>, IAsyncFactory<TProduct, TModel>
    {
        protected AsyncDelegateFactory(Func<TModel, UniTask<TProduct>> factory) : base(factory)
        {
        }
    }
}