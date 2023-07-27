namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UniT.Extensions;

    public static class ServiceProvider
    {
        public static void AddInterfaces(Type type)
        {
            AddInterfaces(Instantiate(type));
        }

        public static void AddInterfaces(object instance)
        {
            instance.GetType().GetInterfaces().ForEach(@interface => Add(@interface, instance));
        }

        public static void AddInterfacesAndSelf(Type type)
        {
            AddInterfacesAndSelf(Instantiate(type));
        }

        public static void AddInterfacesAndSelf(object instance)
        {
            instance.GetType().GetInterfaces().Append(instance.GetType()).ForEach(@interface => Add(@interface, instance));
        }

        public static void Add(Type type)
        {
            Add(type, Instantiate(type));
        }

        public static void Add(object instance)
        {
            Add(instance.GetType(), instance);
        }

        public static void Add(Type type, object instance)
        {
            GetCache(type).Add(instance);
        }

        public static object Get(Type type)
        {
            return GetCache(type).SingleOrDefault() ?? throw new($"No instance found for type {type}");
        }

        public static object[] GetAll(Type type)
        {
            return GetCache(type).ToArray();
        }

        public static object Instantiate(Type type)
        {
            if (type.IsInterface) throw new($"Cannot instantiate interface {type}");
            var ctor = type.GetConstructors().SingleOrDefault()
                       ?? throw new($"Zero or more than one constructor found for type {type}");
            return ctor.Invoke(ResolveParameters(ctor.GetParameters()));
        }

        public static object Invoke(object obj, string methodName)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                         ?? throw new($"No method found with name {methodName} on type {obj.GetType()}");
            return method.Invoke(obj, ResolveParameters(method.GetParameters()));
        }

        #region Generic

        public static void AddInterfaces<T>()
        {
            AddInterfaces(typeof(T));
        }

        public static void AddInterfacesAndSelf<T>()
        {
            AddInterfacesAndSelf(typeof(T));
        }

        public static void Add<T>()
        {
            Add(typeof(T));
        }

        public static void Add<T>(T instance)
        {
            Add(typeof(T), instance);
        }

        public static T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public static T[] GetAll<T>()
        {
            return GetAll(typeof(T)).Cast<T>().ToArray();
        }

        public static T Instantiate<T>()
        {
            return (T)Instantiate(typeof(T));
        }

        #endregion

        #region Private

        private static readonly Dictionary<Type, HashSet<object>> Cache = new();

        private static HashSet<object> GetCache(Type type)
        {
            return Cache.GetOrAdd(type, () => new());
        }

        private static object[] ResolveParameters(ParameterInfo[] parameters)
        {
            return parameters.Select(parameter =>
            {
                var parameterType = parameter.ParameterType;
                if (parameterType.IsArray)
                {
                    var instances = GetAll(parameterType.GetElementType());
                    var instance  = Array.CreateInstance(parameterType.GetElementType()!, instances.Length);
                    instances.CopyTo(instance, 0);
                    return instance;
                }
                else
                {
                    var instance = GetCache(parameterType).SingleOrDefault();
                    if (instance is null && !parameter.HasDefaultValue) throw new($"Cannot resolve type {parameterType} for parameter {parameter.Name}");
                    return instance ?? parameter.DefaultValue;
                }
            }).ToArray();
        }

        #endregion
    }
}