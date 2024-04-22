namespace UniT.Logging
{
    public interface ILoggerFactory
    {
        public ILogger Create(string name);

        #if UNITY_2021_2_OR_NEWER
        public ILogger Create(IHasLogger owner) => this.Create(owner.GetType().Name);
        #endif
    }
}