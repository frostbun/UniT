#if UNIT_DI
#nullable enable
namespace UniT.Instantiator
{
    using System;
    using UniT.DI;
    using UnityEngine.Scripting;

    public sealed class DIInstantiator : IResolver, IInstantiator
    {
        private readonly DependencyContainer container;

        [Preserve]
        public DIInstantiator(DependencyContainer container) => this.container = container;

        object IResolver.Resolve(Type type) => this.container.Get(type);

        T IResolver.Resolve<T>() => this.container.Get<T>();

        object[] IResolver.ResolveAll(Type type) => this.container.GetAll(type);

        T[] IResolver.ResolveAll<T>() => this.container.GetAll<T>();

        object IInstantiator.Instantiate(Type type) => this.container.Instantiate(type);

        T IInstantiator.Instantiate<T>() => this.container.Instantiate<T>();
    }
}
#endif