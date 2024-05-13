namespace UniT.Logging
{
    public interface ILoggerManager
    {
        public ILogger GetLogger(string name);

        public ILogger GetLogger(object owner) => this.GetLogger(owner.GetType().Name);

        public ILogger GetLogger<T>() => this.GetLogger(typeof(T).Name);

        public ILogger GetDefaultLogger() => this.GetLogger(nameof(UniT));
    }
}