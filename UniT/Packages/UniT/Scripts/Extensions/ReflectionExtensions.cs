namespace UniT.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static FieldInfo[] GetAllFields(this Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        {
            return type.GetFields(bindingFlags);
        }

        public static bool IsBackingField(this FieldInfo fieldInfo)
        {
            return fieldInfo.Name.IsBackingFieldName();
        }

        public static bool IsBackingFieldName(this string str)
        {
            return str.StartsWith("<") && str.EndsWith(">k__BackingField");
        }

        public static string ToBackingFieldName(this string str)
        {
            return str.IsBackingFieldName() ? str : $"<{str}>k__BackingField";
        }

        public static string ToPropertyName(this string str)
        {
            return str.IsBackingFieldName() ? str[1..^16] : str;
        }

        public static bool DeriveFromGenericType(this Type type, Type genericType)
        {
            return type.IsGenericType && genericType.IsAssignableFrom(type.GetGenericTypeDefinition());
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
            var fromFieldInfos = from.GetType().GetAllFields();
            var toFieldInfos   = to.GetType().GetAllFields();
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