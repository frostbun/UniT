#if UNIT_DI
namespace UniT.Instantiator
{
    using System;
    using UniT.DI;
    using UnityEngine.Scripting;

    public sealed class DIInstantiator : IInstantiator
    {
        private readonly DependencyContainer dependencyContainer;

        [Preserve]
        public DIInstantiator(DependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        object IInstantiator.Instantiate(Type type) => this.dependencyContainer.Instantiate(type);

        T IInstantiator.Instantiate<T>() => this.dependencyContainer.Instantiate<T>();
    }
}
#endif