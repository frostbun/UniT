namespace UniT.ObjectPool
{
    using System.Threading;

    public interface IRecyclable
    {
        public IObjectPoolManager Manager { get; set; }

        public void OnInstantiate();

        public void OnSpawn();

        public void OnRecycle();

        public CancellationToken GetCancellationTokenOnRecycle();
    }
}