namespace UniT.ObjectPool
{
    public interface IRecyclable
    {
        public IObjectPoolManager Manager { get; protected internal set; }

        protected internal void OnInstantiate();

        protected internal void OnSpawn();

        protected internal void OnRecycle();
    }
}