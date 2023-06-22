namespace UniT.Logging
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public class LoggerManager
    {
        public static readonly LoggerManager Instance = new();

        public Func<ILogger> DefaultLoggerFactory = () => new Logger();

        private readonly Dictionary<Type, ILogger> loggers = new();

        private LoggerManager()
        {
        }

        public void Register<T>(ILogger logger)
        {
            this.loggers.Add(typeof(T), logger);
        }

        public bool Has<T>()
        {
            return this.loggers.ContainsKey(typeof(T));
        }

        public ILogger Get<T>()
        {
            return this.loggers.GetOrAdd(typeof(T), this.DefaultLoggerFactory);
        }
    }
}