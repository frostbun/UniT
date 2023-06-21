namespace UniT.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ServiceProvider<T> where T : class
    {
        private static readonly List<T> _instances = new();

        public static void Add(params T[] instances)
        {
            _instances.AddRange(instances);
        }

        public static T Get()
        {
            return _instances.Count > 0 ? _instances[0] : throw new InvalidOperationException($"No instance of {typeof(T).Name} found");
        }

        public static List<T> GetAll()
        {
            return _instances.ToList();
        }
    }
}