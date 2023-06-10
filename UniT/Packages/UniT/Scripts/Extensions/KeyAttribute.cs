namespace UniT.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class KeyAttribute : Attribute
    {
        public readonly string key;

        public KeyAttribute(string key)
        {
            this.key = key;
        }
    }

    public static class KeyAttributeExtensions
    {
        public static string GetKeyAttribute(this Type type)
        {
            return type.GetCustomAttribute<KeyAttribute>()?.key ?? type.Name;
        }

        public static string[] GetKeyAttributes(this Type type)
        {
            return type.GetCustomAttributes<KeyAttribute>().Select(attr => attr.key).ToArray();
        }
    }
}