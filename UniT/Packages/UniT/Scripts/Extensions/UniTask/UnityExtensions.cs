namespace UniT.Extensions.UniTask
{
    using UnityEngine;

    public static class UnityExtensions
    {
        public static T DontDestroyOnLoad<T>(this T obj) where T : Object
        {
            Object.DontDestroyOnLoad(obj);
            return obj;
        }
    }
}