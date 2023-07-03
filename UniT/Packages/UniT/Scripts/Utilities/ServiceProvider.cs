namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;

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

        public static T[] GetAll()
        {
            return _instances.ToArray();
        }
    }
}