namespace UniT.Factories
{
    public interface IFactory<out TProduct>
    {
        public TProduct Create();
    }

    public interface IFactory<in TParam0, out TProduct>
    {
        public TProduct Create(TParam0 param0);
    }

    public interface IFactory<in TParam0, in TParam1, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8);
    }

    public interface IFactory<in TParam0, in TParam1, in TParam2, in TParam3, in TParam4, in TParam5, in TParam6, in TParam7, in TParam8, in TParam9, out TProduct>
    {
        public TProduct Create(TParam0 param0, TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9);
    }
}