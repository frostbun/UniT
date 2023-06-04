namespace UniT.Extensions
{
    using Unity.Plastic.Newtonsoft.Json;

    public static class Utils
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}