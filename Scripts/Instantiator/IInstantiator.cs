namespace UniT.Instantiator
{
    using System;

    public interface IInstantiator
    {
        public object Instantiate(Type type);

        public T Instantiate<T>();
    }
}