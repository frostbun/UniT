namespace UniT.Logging
{
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    public sealed class UnityLoggerManager : ILoggerManager
    {
        private readonly LogLevel                    level;
        private readonly Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        [Preserve]
        public UnityLoggerManager(LogLevel level)
        {
            this.level = level;
        }

        ILogger ILoggerManager.GetLogger(string name)
        {
            return this.loggers.GetOrAdd(name, () => new UnityLogger(name, new LogConfig { Level = this.level }));
        }
    }
}