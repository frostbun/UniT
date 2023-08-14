namespace UniT.DependencyInjection.Slim
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UniT.Extensions;

    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, HashSet<object>> Cache = new();

        public static void Add<T>(T instance)
        {
            GetCache<T>().Add(instance);
        }

        public static T Get<T>()
        {
            return GetCache<T>().Cast<T>().Single();
        }

        public static T[] GetAll<T>()
        {
            return GetCache<T>().Cast<T>().ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static HashSet<object> GetCache<T>()
        {
            return Cache.GetOrAdd(typeof(T), () => new());
        }
    }
}