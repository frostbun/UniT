namespace UniT.Logging
{
    public interface ILoggerManager
    {
        public ILogger GetLogger(string name);

        #if UNITY_2021_2_OR_NEWER
        public ILogger GetLogger(object owner) => this.GetLogger(owner.GetType().Name);

        public ILogger GetLogger<T>() => this.GetLogger(typeof(T).Name);

        public ILogger GetDefaultLogger() => this.GetLogger(this.GetType().Name);
        #endif
    }
}