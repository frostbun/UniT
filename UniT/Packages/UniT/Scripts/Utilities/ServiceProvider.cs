namespace UniT.Utilities
{
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
            return _instances.Single();
        }

        public static T[] GetAll()
        {
            return _instances.ToArray();
        }
    }
}