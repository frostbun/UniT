#nullable enable
namespace UniT.Instantiator
{
    using System;

    public interface IResolver
    {
        public object Resolve(Type type);

        public T Resolve<T>();

        public object[] ResolveAll(Type type);

        public T[] ResolveAll<T>();
    }
}