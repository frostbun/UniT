namespace UniT.Factories
{
    using System;

    public abstract class DelegateFactory<TProduct> : IFactory<TProduct>
    {
        private readonly Func<TProduct> factory;

        protected DelegateFactory(Func<TProduct> factory) => this.factory = factory;

        public TProduct Create() => this.factory();
    }

    public abstract class DelegateFactory<TParam0, TProduct> : IFactory<TParam0, TProduct>
    {
        private readonly Func<TParam0, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0) => this.factory(param0);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TProduct> : IFactory<TParam0, TParam1, TProduct>
    {
        private readonly Func<TParam0, TParam1, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1) => this.factory(param0, param1);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TProduct> : IFactory<TParam0, TParam1, TParam2, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2) => this.factory(param0, param1, param2);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3) => this.factory(param0, param1, param2, param3);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4) => this.factory(param0, param1, param2, param3, param4);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5) => this.factory(param0, param1, param2, param3, param4, param5);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6) => this.factory(param0, param1, param2, param3, param4, param5, param6);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7) => this.factory(param0, param1, param2, param3, param4, param5, param6, param7);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8) => this.factory(param0, param1, param2, param3, param4, param5, param6, param7, param8);
    }

    public abstract class DelegateFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TProduct> : IFactory<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TProduct>
    {
        private readonly Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TProduct> factory;

        protected DelegateFactory(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TProduct> factory) => this.factory = factory;

        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9) => this.factory(param0, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    }
}