namespace UniT.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class KeyAttribute : Attribute
    {
        public readonly string Key;

        public KeyAttribute(string key)
        {
            this.Key = key;
        }
    }

    public static class KeyAttributeExtensions
    {
        public static string GetKeyAttribute(this Type type)
        {
            return type.GetCustomAttribute<KeyAttribute>()?.Key ?? type.Name;
        }

        public static string[] GetKeyAttributes(this Type type)
        {
            return type.GetCustomAttributes<KeyAttribute>().Select(attr => attr.Key).ToArray();
        }
    }
}