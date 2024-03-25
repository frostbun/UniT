#if UNIT_DI
namespace UniT.Instantiator
{
    using System;
    using UniT.DI;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class DIInstantiator : IInstantiator
    {
        private readonly DependencyContainer dependencyContainer;

        public DIInstantiator(DependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        object IInstantiator.Instantiate(Type type) => this.dependencyContainer.Instantiate(type);

        T IInstantiator.Instantiate<T>() => this.dependencyContainer.Instantiate<T>();
    }
}
#endif