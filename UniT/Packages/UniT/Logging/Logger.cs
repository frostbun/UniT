namespace UniT.Logging
{
    using System;
    using UniT.Extensions;
    using UnityEngine;

    public class Logger : ILogger
    {
        private readonly LogConfig config;

        public Logger(LogConfig config)
        {
            this.config = config;
            this.Log($"{this.GetType().Name} instantiated with config: {config.ToJson()}", Color.green);
        }

        public void Log(string message, Color? color = null)
        {
            if (!this.config.logLevel.HasFlag(LogLevel.Log)) return;
            Debug.Log(message.WithColor(color));
        }

        public void Warning(string message, Color? color = null)
        {
            if (!this.config.logLevel.HasFlag(LogLevel.Warning)) return;
            Debug.LogWarning(message.WithColor(color));
        }

        public void Error(string message, Color? color = null)
        {
            if (!this.config.logLevel.HasFlag(LogLevel.Error)) return;
            Debug.LogError(message.WithColor(color));
        }

        public void Exception(Exception exception)
        {
            if (!this.config.logLevel.HasFlag(LogLevel.Exception)) return;
            Debug.LogException(exception);
        }
    }
}