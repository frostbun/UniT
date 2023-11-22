namespace UniT.Factories
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class AsyncDelegateFactory<TProduct> : IAsyncFactory<TProduct>
    {
        private readonly Func<UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync() => this.factory();
    }

    public abstract class AsyncDelegateFactory<TParam0, TProduct> : IAsyncFactory<TParam0, TProduct>
    {
        private readonly Func<TParam0, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0) => this.factory(param0);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TProduct> : IAsyncFactory<TParam0, TParam1, TProduct>
    {
        private readonly Func<TParam0, TParam1, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1) => this.factory(param0, param1);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2) => this.factory(param0, param1, param2);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3) => this.factory(param0, param1, param2, param3);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) => this.factory(param0, param1, param2, param3, param4);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5) => this.factory(param0, param1, param2, param3, param4, param5);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6) => this.factory(param0, param1, param2, param3, param4, param5, param6);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7) => this.factory(param0, param1, param2, param3, param4, param5, param6, param7);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8) => this.factory(param0, param1, param2, param3, param4, param5, param6, param7, param8);
    }

    public abstract class AsyncDelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TProduct> : IAsyncFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, UniTask<TProduct>> factory;

        protected AsyncDelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, UniTask<TProduct>> factory) => this.factory = factory;

        public UniTask<TProduct> CreateAsync(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9) => this.factory(param0, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    }
}