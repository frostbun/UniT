namespace UniT.Extensions
{
    using UnityEngine;

    public static class UnityExtensions
    {
        public static T DontDestroyOnLoad<T>(this T obj) where T : Object
        {
            Object.DontDestroyOnLoad(obj);
            return obj;
        }

        public static Sprite CreateSprite(this Texture2D texture, Vector2? pivot = null)
        {
            return Sprite.Create(texture, new(0, 0, texture.width, texture.height), pivot ?? Vector2.zero);
        }
    }
}