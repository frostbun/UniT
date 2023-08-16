namespace UniT.ObjectPool
{
    public interface IRecyclable
    {
        public IObjectPoolManager Manager { get; set; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();
    }
}