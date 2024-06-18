#nullable enable
namespace UniT.Logging
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    public sealed class UnityLoggerManager : ILoggerManager
    {
        private readonly LogLevel level;

        private readonly Dictionary<string, ILogger> loggers = new Dictionary<string, ILogger>();

        [Preserve]
        public UnityLoggerManager(LogLevel level = LogLevel.Info)
        {
            this.level = level;
        }

        ILogger ILoggerManager.GetLogger(string name)
        {
            return this.loggers.GetOrAdd(name, () => new UnityLogger(name, new LogConfig { Level = this.level }));
        }

        IEnumerable<ILogger> ILoggerManager.GetAllLoggers()
        {
            return this.loggers.Values;
        }

        private sealed class UnityLogger : Logger
        {
            public UnityLogger(string name, LogConfig config) : base(name, config)
            {
            }

            protected override void Debug(string message)
            {
                UnityEngine.Debug.Log(message);
            }

            protected override void Info(string message)
            {
                UnityEngine.Debug.Log(message);
            }

            protected override void Warning(string message)
            {
                UnityEngine.Debug.LogWarning(message);
            }

            protected override void Error(string message)
            {
                UnityEngine.Debug.LogError(message);
            }

            protected override void Critical(string message)
            {
                UnityEngine.Debug.LogError(message);
            }

            protected override void Exception(string message, Exception exception)
            {
                UnityEngine.Debug.LogError(message);
                UnityEngine.Debug.LogException(exception);
            }
        }
    }
}