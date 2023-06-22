namespace UniT.Logging
{
    using System;
    using UnityEngine;

    public abstract class BaseLogger : ILogger
    {
        public LogConfig Config { get; } = new();

        void ILogger.Debug(string message, Color? color)
        {
            if (this.Config.Level > LogLevel.Debug) return;
            this.Debug(message, color);
        }

        void ILogger.Info(string message, Color? color)
        {
            if (this.Config.Level > LogLevel.Info) return;
            this.Info(message, color);
        }

        void ILogger.Warning(string message, Color? color)
        {
            if (this.Config.Level > LogLevel.Warning) return;
            this.Warning(message, color);
        }

        void ILogger.Error(string message, Color? color)
        {
            if (this.Config.Level > LogLevel.Error) return;
            this.Error(message, color);
        }

        void ILogger.Critical(string message, Color? color)
        {
            if (this.Config.Level > LogLevel.Critical) return;
            this.Critical(message, color);
        }

        void ILogger.Exception(Exception exception)
        {
            if (this.Config.Level > LogLevel.Exception) return;
            this.Exception(exception);
        }

        protected abstract void Debug(string message, Color? color = null);

        protected abstract void Info(string message, Color? color = null);

        protected abstract void Warning(string message, Color? color = null);

        protected abstract void Error(string message, Color? color = null);

        protected abstract void Critical(string message, Color? color = null);

        protected abstract void Exception(Exception exception);
    }
}