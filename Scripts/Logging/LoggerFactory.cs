namespace UniT.Logging
{
    using System;
    using UnityEngine.Scripting;

    public sealed class LoggerFactory : ILoggerFactory
    {
        private readonly Func<LogConfig> configFactory;

        [Preserve]
        public LoggerFactory(Func<LogConfig> configFactory)
        {
            this.configFactory = configFactory;
        }

        ILogger ILoggerFactory.Create(string name)
        {
            return new UnityLogger(name, this.configFactory());
        }
    }
}