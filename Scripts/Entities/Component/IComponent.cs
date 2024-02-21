namespace UniT.Entities
{
    using UnityEngine;

    public interface IComponent
    {
        public IEntityManager Manager { get; set; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        public Transform transform { get; }
    }
}