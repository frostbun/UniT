#if UNIT_ZENJECT
#nullable enable
namespace UniT.Instantiator
{
    using System;
    using System.Linq;
    using UnityEngine.Scripting;
    using Zenject;

    public sealed class ZenjectInstantiator : IResolver, IInstantiator
    {
        private readonly DiContainer container;

        [Preserve]
        public ZenjectInstantiator(DiContainer container) => this.container = container;

        object IResolver.Resolve(Type type) => this.container.Resolve(type);

        T IResolver.Resolve<T>() => this.container.Resolve<T>();

        object[] IResolver.ResolveAll(Type type) => this.container.ResolveAll(type).Cast<object>().ToArray();

        T[] IResolver.ResolveAll<T>() => this.container.ResolveAll<T>().ToArray();

        object IInstantiator.Instantiate(Type type) => this.container.Instantiate(type);

        T IInstantiator.Instantiate<T>() => this.container.Instantiate<T>();
    }
}
#endif