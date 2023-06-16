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
            this.Info($"{nameof(Logger)} instantiated with config: {config.ToJson()}", Color.green);
        }

        public void Debug(string message, Color? color = null)
        {
            if (this.config.logLevel > LogLevel.Debug) return;
            UnityEngine.Debug.Log(message.WithColor(color));
        }

        public void Info(string message, Color? color = null)
        {
            if (this.config.logLevel > LogLevel.Info) return;
            UnityEngine.Debug.Log(message.WithColor(color));
        }

        public void Warning(string message, Color? color = null)
        {
            if (this.config.logLevel > LogLevel.Warning) return;
            UnityEngine.Debug.LogWarning(message.WithColor(color));
        }

        public void Error(string message, Color? color = null)
        {
            if (this.config.logLevel > LogLevel.Error) return;
            UnityEngine.Debug.LogError(message.WithColor(color));
        }

        public void Critical(string message, Color? color = null)
        {
            if (this.config.logLevel > LogLevel.Critical) return;
            UnityEngine.Debug.LogError(message.WithColor(color));
        }

        public void Exception(Exception exception)
        {
            if (this.config.logLevel > LogLevel.Exception) return;
            UnityEngine.Debug.LogException(exception);
        }
    }
}