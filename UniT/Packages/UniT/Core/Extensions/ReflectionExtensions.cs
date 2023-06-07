namespace UniT.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static string GetKeyAttribute(this Type type)
        {
            return type.GetCustomAttribute<KeyAttribute>()?.key ?? type.Name;
        }

        public static string[] GetKeyAttributes(this Type type)
        {
            return type.GetCustomAttributes<KeyAttribute>().Select(attr => attr.key).ToArray();
        }

        public static IEnumerable<Type> GetDerivedTypes(this Type baseType, bool sameAssembly = false)
        {
            var baseAsm = Assembly.GetAssembly(baseType);
            return AppDomain.CurrentDomain.GetAssemblies()
                            .Where(asm => !asm.IsDynamic && (!sameAssembly || asm == baseAsm))
                            .SelectMany(asm => asm.GetTypes())
                            .Where(type => type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type));
        }

        public static void CopyTo(this object from, object to)
        {
            var fromFieldInfos = from.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var toFieldInfos   = to.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var fromField in fromFieldInfos)
            {
                var toField = toFieldInfos.FirstOrDefault(toField => toField.Name == fromField.Name && toField.FieldType.IsAssignableFrom(fromField.FieldType));
                if (toField != null)
                {
                    toField.SetValue(to, fromField.GetValue(from));
                }
            }
        }
    }
}