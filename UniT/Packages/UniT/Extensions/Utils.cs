namespace UniT.Extensions
{
    using Newtonsoft.Json;
    using UnityEngine;

    public static class Utils
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string ToHex(this Color color)
        {
            return $"{(byte)(color.r * 255f):X2}{(byte)(color.g * 255f):X2}{(byte)(color.b * 255f):X2}";
        }
    }
}