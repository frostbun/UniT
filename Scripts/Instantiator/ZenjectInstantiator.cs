﻿#if UNIT_ZENJECT
namespace UniT.Instantiator
{
    using System;
    using UnityEngine.Scripting;

    [Preserve]
    public sealed class ZenjectInstantiator : IInstantiator
    {
        private readonly Zenject.IInstantiator instantiator;

        public ZenjectInstantiator(Zenject.IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        object IInstantiator.Instantiate(Type type) => this.instantiator.Instantiate(type);

        T IInstantiator.Instantiate<T>() => this.instantiator.Instantiate<T>();
    }
}
#endif