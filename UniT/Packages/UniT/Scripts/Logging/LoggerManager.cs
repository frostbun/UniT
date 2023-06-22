namespace UniT.Logging
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public class LoggerManager
    {
        public static readonly LoggerManager Instance = new();

        private readonly Dictionary<Type, ILogger> loggers = new();

        private LoggerManager()
        {
        }

        public void Register<T>(ILogger logger)
        {
            this.loggers.Add(typeof(T), logger);
        }

        public ILogger Get<T>()
        {
            return this.loggers.GetOrDefault(typeof(T));
        }
    }
}