namespace UniT.Pooling
{
    using UnityEngine;

    public interface IRecyclable
    {
        public IObjectPoolManager Manager { get; set; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        public GameObject gameObject { get; }
    }

    public static class RecyclableExtensions
    {
        public static void Recycle(this IRecyclable component)
        {
            component.Manager.Recycle(component.gameObject);
        }
    }
}