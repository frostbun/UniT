namespace UniT.Extensions
{
    using Newtonsoft.Json;
    using UnityEngine;

    public static class StringExtensions
    {
        public static bool IsNullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Wrap(this string str, string wrapper)
        {
            return str.Wrap(wrapper, wrapper);
        }

        public static string Wrap(this string str, string prefix, string suffix)
        {
            return $"{prefix}{str}{suffix}";
        }

        public static string WithColor(this string str, Color? color)
        {
            return color is { } c ? $"<color=#{c.ToHex()}>{str}</color>" : str;
        }

        public static string ToHex(this Color color)
        {
            return $"{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}";
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}