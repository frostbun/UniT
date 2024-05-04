#if UNIT_VCONTAINER
namespace UniT.Instantiator
{
    using System;
    using VContainer;
    using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;

    public sealed class VContainerInstantiator : IInstantiator
    {
        private readonly IObjectResolver resolver;

        [Preserve]
        public VContainerInstantiator(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        object IInstantiator.Instantiate(Type type)
        {
            var scope = this.resolver.CreateScope(builder => builder.Register(type, Lifetime.Singleton));
            return scope.Resolve(type);
        }

        T IInstantiator.Instantiate<T>()
        {
            var scope = this.resolver.CreateScope(builder => builder.Register<T>(Lifetime.Singleton));
            return scope.Resolve<T>();
        }
    }
}
#endif