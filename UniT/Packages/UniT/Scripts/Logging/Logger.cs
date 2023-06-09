namespace UniT.Logging
{
    using System;

    public class Logger : BaseLogger
    {
        public Logger(string name, LogConfig config = null) : base(name, config)
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