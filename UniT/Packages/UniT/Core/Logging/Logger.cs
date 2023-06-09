namespace UniT.Core.Logging
{
    using System;
    using UniT.Core.Extensions;
    using UnityEngine;

    public class Logger : ILogger
    {
        private readonly LogConfig config;

        public Logger(LogConfig config)
        {
            this.config = config;
            this.Info($"{this.GetType().Name} instantiated with config: {config.ToJson()}", Color.green);
        }

        public void Debug(string message, Color? color = null)
        {
            if ((this.config.logLevel & LogLevel.Debug) != 0) return;
            UnityEngine.Debug.Log(message.WithColor(color));
        }

        public void Info(string message, Color? color = null)
        {
            if ((this.config.logLevel & LogLevel.Info) != 0) return;
            UnityEngine.Debug.Log(message.WithColor(color));
        }

        public void Warning(string message, Color? color = null)
        {
            if ((this.config.logLevel & LogLevel.Warning) != 0) return;
            UnityEngine.Debug.LogWarning(message.WithColor(color));
        }

        public void Error(string message, Color? color = null)
        {
            if ((this.config.logLevel & LogLevel.Error) != 0) return;
            UnityEngine.Debug.LogError(message.WithColor(color));
        }

        public void Critical(string message, Color? color = null)
        {
            if ((this.config.logLevel & LogLevel.Critical) != 0) return;
            UnityEngine.Debug.LogError(message.WithColor(color));
        }

        public void Exception(Exception exception)
        {
            if ((this.config.logLevel & LogLevel.Exception) != 0) return;
            UnityEngine.Debug.LogException(exception);
        }
    }
}