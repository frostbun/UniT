#if UNIT_DI
#nullable enable
namespace UniT.Instantiator
{
    using System;
    using UniT.DI;
    using UnityEngine.Scripting;

    public sealed class DIInstantiator : IInstantiator
    {
        private readonly DependencyContainer container;

        [Preserve]
        public DIInstantiator(DependencyContainer container) => this.container = container;

        object IInstantiator.Instantiate(Type type) => this.container.Instantiate(type);

        T IInstantiator.Instantiate<T>() => this.container.Instantiate<T>();
    }
}
#endif