namespace UniT.Utils
{
    public interface IFactory<out TProduct>
    {
        public TProduct Create();
    }

    public interface IFactory<out TProduct, in TModel>
    {
        public TProduct Create(TModel model);
    }
}